using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;

namespace PopOptBox.Optimisers.EvolutionaryComputation.ParentSelection
{
    /// <summary>
    /// A parent selection operator that selects two or more parents for recombination using greedy selection of the fittest individuals.
    /// </summary>
    public class ParentSelectionGreedy : Operator, IParentSelectionOperator
    {
        /// <summary>
        /// Creates a greedy parent selection operator.
        /// </summary>
        public ParentSelectionGreedy() : base ("Greedy")
        {
        }

        /// <summary>
        /// Gets a subset of the population with the best Fitness.
        /// </summary>
        /// <param name="population">The <see cref="Population"/> to be selected from.</param>
        /// <param name="numberToSelect">The number of individuals to parents to select.</param>
        /// <returns>A list of <see cref="Individual"/>s selected.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when population is smaller than the number of individuals requested.</exception>
        public IEnumerable<Individual> Select(Population population, int numberToSelect)
        {
            if (numberToSelect > population.Count)
                throw new ArgumentOutOfRangeException(nameof(numberToSelect),
                    "The population is too small to select this many individuals.");

            return population.Take(numberToSelect);
        }
    }
}