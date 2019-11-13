using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;
using MathNet.Numerics.Statistics;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// The PCX operator (from Deb 2002)
    /// </summary>
    public class RecombinationParentCentric : Operator, IRecombinationOperator
    {
        private readonly double sigmaEta;
        private readonly double sigmaZeta;
        private readonly RandomNumberManager rngManager;

        /// <summary>
        /// Creates a new PCX operator for Evolutionary Algorithm recombination
        /// </summary>
        /// <param name="sigmaEta">Gain parallel to direction of search.</param>
        /// <param name="sigmaZeta">Gain perpendicular to direction of search.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="sigmaEta"/> or <see cref="sigmaZeta"/> are not greater than zero.</exception>
        public RecombinationParentCentric(
            double sigmaEta = 0.1, 
            double sigmaZeta = 0.1) 
            : base($"PCX (sigma_eta = {sigmaEta.ToString("F2", System.Globalization.NumberFormatInfo.InvariantInfo)}, " + 
                  $"sigma_zeta = {sigmaZeta.ToString("F2", System.Globalization.NumberFormatInfo.InvariantInfo)})")
        {
            if (sigmaEta <= 0)
                throw new ArgumentOutOfRangeException(nameof(sigmaEta), "Sigma_eta must be greater than zero.");
            this.sigmaEta = sigmaEta;

            if (sigmaZeta <= 0)
                throw new ArgumentOutOfRangeException(nameof(sigmaZeta), "Sigma_eta must be greater than zero.");
            this.sigmaZeta = sigmaZeta;

            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a new Decision Vector, based on the PCX logic.
        /// </summary>
        /// <param name="parents">A list of parent <see cref="DecisionVector"/>s.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if:
        /// - there are less than two parents; or
        /// - the parents have different length or zero length decision vectors; or
        /// - any of the parents have non-continuous Decision Vector elements.
        /// </exception>
        public DecisionVector Operate(params DecisionVector[] parents)
        {
            if (parents.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(parents),
                    "There must be at least two parents.");

            // TODO: These calls to .Any() are slow - can we remove the error checking?
            if (parents.Any(p => p.GetContinuousElements().Count == 0))
                throw new ArgumentOutOfRangeException(nameof(parents),
                    "Parents must have non-zero length decision vectors.");

            if (parents.Any(p => p.GetContinuousElements().Count != parents.First().Count))
                throw new ArgumentOutOfRangeException(nameof(parents),
                    "Parents must have the same length and fully continuous decision vectors.");

            // 1: Pre-process
            var parentDVs = Matrix<double>.Build.DenseOfColumns(parents.Select(dv => dv.Select(d => (double)d)));
            var motherDV = Vector<double>.Build.DenseOfArray(parents.ElementAt(0).Select(d => (double)d).ToArray());

            // 1a: centroid of all parents
            var centroid = parentDVs.RowSums().Divide(parents.Count());

            // 1b: vector distance from centroid to mother (following Deb's C code, not paper)
            var motherCentroidVectorDistance = centroid - motherDV;
            var motherCentroidAbsoluteDistance = motherCentroidVectorDistance.L2Norm();
            if (motherCentroidAbsoluteDistance < 1e-20)
                return DecisionVector.CreateForEmpty();
            
            // 1c: vector distance from other parents to mother
            var otherParentDVs = parentDVs.RemoveColumn(0);
            var parentMotherVectorDistances = otherParentDVs.EnumerateColumns()
                .Select(v => v - motherDV).ToArray();
            var parentMotherAbsoluteDistances = parentMotherVectorDistances.Select(v => v.L2Norm()).ToArray();
            if (parentMotherAbsoluteDistances.Any(d => d < 1e-20))
                return DecisionVector.CreateForEmpty();

            // 1d: perpendicular distances from other parents to centroid-mother vector
            var orthogonalDistances = parentMotherVectorDistances
                .Select((v,i) => parentMotherAbsoluteDistances.ElementAt(i) * 
                                 Math.Sqrt(1.0 - Math.Pow(
                                               v.DotProduct(motherCentroidVectorDistance) /
                                               (parentMotherAbsoluteDistances.ElementAt(i) * motherCentroidAbsoluteDistance), 
                                               2.0)));
            var meanOrthogonalDistance = orthogonalDistances.Mean();

            // 2: Now create a new individual
            var normRnd = new MathNet.Numerics.Distributions.Normal(rngManager.Rng);
            var samplesEta = new double[motherDV.Count];
            normRnd.Samples(samplesEta);

            var newRandomDv = Vector<double>.Build.DenseOfArray(samplesEta)
                .Multiply(sigmaEta * meanOrthogonalDistance);

            //Remove component of randomness in direction of ?
            var offset1 = motherCentroidVectorDistance
                .Multiply(newRandomDv.DotProduct(motherCentroidVectorDistance))
                .Divide(Math.Pow(motherCentroidAbsoluteDistance, 2.0));
            newRandomDv -= offset1;

            var offset2 = motherCentroidVectorDistance
                .Multiply(sigmaZeta * normRnd.Sample());
            newRandomDv += offset2;

            // Modification of Deb2002 which should maintain stability.
            var finalDv = motherDV + 
                          newRandomDv.Divide(Math.Sqrt(motherDV.Count));

            return DecisionVector.CreateFromArray(parents.First().GetDecisionSpace(), finalDv.ToArray());
        }
    }
}
