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
        /// Gets a new Decision Vector, based on simulating the operation of a single-point binary crossover.
        /// </summary>
        /// <param name="parents">Two <see cref="DecisionVector"/>s to use as a parents.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        public DecisionVector Operate(params DecisionVector[] parents)
        {
            var firstParent = parents[0];
            var secondParent = parents[1];
            
            if (firstParent.GetContinuousElements().Count == 0)
                throw new ArgumentOutOfRangeException(nameof(firstParent),
                    "Parents must have non-zero length continuous Decision Vector.");

            if (secondParent.GetContinuousElements().Count == 0)
                throw new ArgumentOutOfRangeException(nameof(secondParent),
                    "Parents must have non-zero length continuous Decision Vector.");

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
            return u < 0.5 
                ? Math.Exp(Math.Log(u / 0.5) / (n + 1)) 
                : Math.Exp(Math.Log(0.5 / (1 - u)) / (n + 1));
        }
    }
}
