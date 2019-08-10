using System;
using System.Collections.Generic;
using System.Linq;

namespace PopOptBox.Base.Helpers
{
    /// <summary>
    /// Solution Vector to Fitness conversion for resolving multiple objectives to a single objective
    /// through calculating the distance to a predefined point.
    /// <see cref="Management.Optimiser"/>.
    /// </summary>
    public class SolutionToFitnessDistanceToPoint
    {
        private readonly double[] optimalPoint;

        /// <summary>
        /// Create a weighting function which converts multiple objectives to one objective.
        /// </summary>
        /// <param name="optimalPoint">The location from which the distance should be measured.</param>
        public SolutionToFitnessDistanceToPoint(IEnumerable<double> optimalPoint)
        {
            this.optimalPoint = optimalPoint.ToArray();
        }

        /// <summary>
        /// Calculate average distance to the optimal point to return a single objective fitness.
        /// </summary>
        /// <param name="solution">The Solution Vector.</param>
        /// <returns>The Fitness.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the optimal point vector and solution vector are different lengths.</exception>
        public double Weight(double[] solution)
        {
            if (solution.Length != optimalPoint.Length)
                throw new System.ArgumentOutOfRangeException(nameof(solution),
                    "The number of dimensions does not equal the length of the optimal point vector.");

            return Math.Sqrt(
                solution.Select((x, i) => Math.Pow(x - optimalPoint[i], 2.0))
                .Sum());
        }
    }
}