using Optimisation.Base.Variables;
using System;
using System.Linq;
using MathNet.Numerics.Random;
using MathNet.Numerics.LinearAlgebra;

namespace Optimisation.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Performs the Simulated Binary Crossover (SBX) as proposed by Deb and Agrawal (1995)
    /// </summary>
    public class CrossoverSimulatedBinary : Operator, ITwoParentCrossoverOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly int n;

        /// <summary>
        /// Constructs a crossover operator to perform simulated binary crossover on real-valued decision vectors.
        /// </summary>
        /// <param name="n">The expansion parameter (lower values create children further from the parents).</param>
        public CrossoverSimulatedBinary(int n) 
            : base($"Simulated Binary (beta = {n.ToString()}")
        {
            if (n < 0)
                throw new System.ArgumentOutOfRangeException(nameof(n),
                    "Beta should not be negative.");
            this.n = n;

            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a new decision vector, based on simulating the operation of a single-point binary crossover.
        /// </summary>
        /// <param name="firstParent">One <see cref="DecisionVector"/> to use as a parent.</param>
        /// <param name="secondParent">Another <see cref="DecisionVector"/> to use as a parent.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        public DecisionVector Operate(DecisionVector firstParent, DecisionVector secondParent)
        {
            if (firstParent.GetContinuousElements().Vector.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(firstParent),
                    "Parents must have non-zero length continuous decision vector.");

            if (secondParent.GetContinuousElements().Vector.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(secondParent),
                    "Parents must have non-zero length continuous decision vector.");

            // Multiply by -1 randomly to ensure either possibility is equally possible.
            var multiplier = rngManager.Rng.NextBoolean() ? -1 : 1;

            // Get beta value
            var beta = getBetaFromU(rngManager.Rng.NextDouble());

            var mean = Vector<double>.Build.DenseOfArray(
                (firstParent.GetContinuousElements() + secondParent.GetContinuousElements()).ToArray()) 
                * 0.5;
            var offset = Vector<double>.Build.DenseOfArray(
                (secondParent.GetContinuousElements() - firstParent.GetContinuousElements())
                .Select(e => Math.Abs(e)).ToArray())
                * multiplier * 0.5 * beta;

            return DecisionVector.CreateFromArray(firstParent.GetDecisionSpace(),
                (mean + offset).ToArray());
        }

        private double getBetaFromU(double u)
        {
            if (u == 0.5)
                return 1;
            else if (u < 0.5)
            {
                return Math.Exp(Math.Log(u / 0.5) / (n + 1));
            }
            else
            {
                return Math.Exp(Math.Log(0.5 / (1 - u)) / (n + 1));
            }
        }
    }
}
