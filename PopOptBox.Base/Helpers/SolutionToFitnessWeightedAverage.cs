using System.Collections.Generic;
using System.Linq;

namespace PopOptBox.Base.Helpers
{
    /// <summary>
    /// Solution Vector to Fitness conversion for resolving multiple objectives to a single objective
    /// through applying a weighted average.
    /// <see cref="Management.Optimiser"/>.
    /// </summary>
    public class SolutionToFitnessWeightedAverage
    {
        private readonly double[] weights;

        /// <summary>
        /// Create a weighting function which converts multiple objectives to one objective.
        /// </summary>
        /// <param name="weights">
        /// The weights to multiply each objective by. 
        /// Dimensions to be maximised should have a negative value.
        /// </param>
        public SolutionToFitnessWeightedAverage(IEnumerable<double> weights)
        {
            this.weights = weights.ToArray();
        }

        /// <summary>
        /// Calculate weighted sum of objectives to return a single objective fitness.
        /// </summary>
        /// <param name="solution">The Solution Vector.</param>
        /// <returns>The Fitness.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the weights vector and solution vector are different lengths.</exception>
        public double Weight(double[] solution)
        {
            if (solution.Length != weights.Length)
                throw new System.ArgumentOutOfRangeException(nameof(solution),
                    "The number of dimensions does not equal the length of the weight vector.");

            return solution
                .Select((x, i) => x * weights[i])
                .Sum();
        }
    }
}