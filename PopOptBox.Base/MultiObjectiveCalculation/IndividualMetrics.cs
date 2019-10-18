using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.MultiObjectiveCalculation
{
    public static class IndividualMetrics
    {
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
    }
}