using System;
using System.Linq;
using MathNet.Numerics.Random;
using MathNet.Numerics.LinearAlgebra;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Performs the Simulated Binary Crossover (SBX) as proposed by Deb and Agrawal (1995)
    /// </summary>
    public class CrossoverSimulatedBinary : Operator, IRecombinationOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly int eta;

        /// <summary>
        /// Constructs a crossover operator to perform simulated binary crossover on real-valued decision vectors.
        /// </summary>
        /// <param name="eta">The expansion parameter (lower values create children further from the parents).</param>
        public CrossoverSimulatedBinary(int eta) 
            : base($"Simulated Binary (eta = {eta.ToString()})")
        {
            if (eta < 0)
                throw new System.ArgumentOutOfRangeException(nameof(eta),
                    "Eta should not be negative.");
            this.eta = eta;

            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a new Decision Vector, based on simulating the operation of a single-point binary crossover.
        /// </summary>
        /// <param name="parents">Two <see cref="DecisionVector"/>s to use as a parents.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        public DecisionVector Operate(params DecisionVector[] parents)
        {
            if (parents[0].GetContinuousElements().Count == 0)
                throw new ArgumentOutOfRangeException(nameof(parents),
                    "Parents must have non-zero length continuous Decision Vector.");

            if (parents[1].GetContinuousElements().Count == 0)
                throw new ArgumentOutOfRangeException(nameof(parents),
                    "Parents must have non-zero length continuous Decision Vector.");

            var firstParent = Vector<double>.Build.DenseOfEnumerable(
                parents[0].GetContinuousElements().Select(d => (double)d));
            var secondParent = Vector<double>.Build.DenseOfEnumerable(
                parents[1].GetContinuousElements().Select(d => (double)d));

            // Multiply by -1 randomly to ensure either possibility is equally possible.
            var multiplier = rngManager.Rng.NextBoolean() ? -1 : 1;

            // Get beta value
            var beta = getBetaFromU(rngManager.Rng.NextDouble());

            // Create child
            var child = 0.5 * ((1 - beta * multiplier) * firstParent
                                + (1 + beta * multiplier) * secondParent);
            
            return DecisionVector.CreateFromArray(parents[0].GetDecisionSpace(),
                child.ToArray());
        }

        private double getBetaFromU(double u)
        {
            return u <= 0.5 
                ? Math.Exp(Math.Log(2.0 * u) / (eta + 1)) 
                : Math.Exp(Math.Log(0.5 / (1 - u)) / (eta + 1));
        }
    }
}
