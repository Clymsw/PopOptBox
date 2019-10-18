using System.Linq;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.MultiObjectiveCalculation
{
    public static class PopulationMetrics
    {
        /// <summary>
        /// Gets the individuals on the specified Pareto Front.
        /// </summary>
        /// <param name="population">The population extended by this method.</param>
        /// <param name="front">The desired Pareto Front (1 is the first, by tradition).</param>
        /// <returns>An array of <see cref="Individual"/>s.</returns>
        public static Individual[] ParetoFront(this Population population, int front = 1)
        {
            return population.Best().SolutionVector.Length > 1
                ? population
                    .Where(i => i.GetProperty<int>(OptimiserPropertyNames.ParetoFront) == front).ToArray()
                : front < population.Count
                    ? new[] { population[front - 1] }
                    : new Individual[0];
        }

        /// <summary>
        /// Gets all non-dominated individuals.
        /// </summary>
        /// <remarks>
        /// Should be equivalent to population.ParetoFront(1),
        /// but depends on different property (from <see cref="OptimiserPropertyNames"/>).
        /// </remarks>
        /// <param name="population">The population extended by this method.</param>
        /// <returns>An array of <see cref="Individual"/>s.</returns>
        public static Individual[] NonDominatedSet(this Population population)
        {
            return population.Best().SolutionVector.Length > 1
                ? population
                    .Where(i => i.NonDominationRank() == 0).ToArray()
                : new[] { population.Best() };
        }
    }
}