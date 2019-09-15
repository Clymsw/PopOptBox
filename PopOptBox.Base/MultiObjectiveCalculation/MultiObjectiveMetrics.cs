using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.MultiObjectiveCalculation
{
    public static class MultiObjectiveMetrics
    {
        #region Individual

        /// <summary>
        /// Gets the non-domination rank of an individual (the number of individuals dominating it).
        /// </summary>
        /// <remarks>This is subtly different from a Pareto Front rank.</remarks>
        /// <param name="individual">individuals</param>
        /// <returns>The rank.</returns>
        public static int NonDominationRank(this Individual individual)
        {
            return individual.GetProperty<List<Individual>>(OptimiserPropertyNames.DominatedBy).Count;
        }

        /// <summary>
        /// Gets whether an <see cref="Individual"/> dominates another one.
        /// This is defined as:
        ///  - for all objectives, the solution values are equal or worse, and
        ///  - for at least one objective, the solution value is worse. 
        /// </summary>
        /// <param name="individual">The individual extended by this method.</param>
        /// <param name="other">The individual to compare to.</param>
        /// <param name="minimise">An array of the same length as the Solution Vectors, <see langword="true"/> if that objective is to be minimised.</param>
        /// <returns><see langword="true"/> if the other individual is dominated.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the two Solution Vectors have different lengths.</exception>
        public static bool IsDominating(this Individual individual, Individual other, bool[] minimise)
        {
            if (other.SolutionVector.Length != individual.SolutionVector.Length)
                throw new InvalidOperationException(
                    "Other individual must have the same number of objectives in its Solution Vector.");

            if (individual.SolutionVector.Length != minimise.Length)
                throw new ArgumentOutOfRangeException(nameof(minimise),
                    "Minimisation instruction must be same length as the Solution Vector.");

            var sol1 = individual.SolutionVector.Select((v, i) => v * (minimise[i] ? 1 : -1)).ToArray();
            var sol2 = other.SolutionVector.Select((v, i) => v * (minimise[i] ? 1 : -1)).ToArray();

            if (sol1.Select((v, i) => v < sol2.ElementAt(i)).Any(b => b))
                if (sol1.Select((v, i) => v <= sol2.ElementAt(i)).All(b => b))
                    return true;

            return false;
        }

        #endregion

        #region Individuals

        /// <summary>
        /// Implements the Crowding Distance algorithm proposed by Deb et al. (2002).
        /// Note that the individuals provided should be on the same Pareto Front.
        /// Calculates the crowding distance and assigns it into the individual based on the property name provided.
        /// </summary>
        /// <param name="individuals">Individuals forming a Pareto Front.</param>
        /// <param name="propertyName">Name to give to the property inserted into the <see cref="Individual"/>s.</param>
        public static void AssignCrowdingDistance(IEnumerable<Individual> individuals, string propertyName)
        {
            var inds = individuals as Individual[] ?? individuals.ToArray();
            for (var m = 0; m < inds[0].SolutionVector.Length; m++)
            {
                var tempSorted = inds.OrderBy(a => a.SolutionVector.ElementAt(m)).ToArray();
                var fmin = tempSorted.First().SolutionVector[m];
                tempSorted.First().SetProperty(propertyName, double.MaxValue);
                var fmax = tempSorted.Last().SolutionVector[m];
                tempSorted.Last().SetProperty(propertyName, double.MaxValue);

                for (var i = 1; i < inds.Length - 1; i++)
                {
                    var distance = 0.0;
                    if (m > 0)
                        distance = inds[i].GetProperty<double>(propertyName);

                    if (distance == double.MaxValue)
                        continue;

                    tempSorted.ElementAt(i).SetProperty(propertyName,
                        distance +
                        (tempSorted.ElementAt(i + 1).SolutionVector.ElementAt(m) -
                         tempSorted.ElementAt(i - 1).SolutionVector.ElementAt(m)) /
                        (fmax - fmin));
                }
            }
        }

        #endregion

        #region Population

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

        #endregion
    }
}