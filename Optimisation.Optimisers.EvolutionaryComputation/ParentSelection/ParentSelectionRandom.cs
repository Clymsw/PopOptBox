using System;
using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Management;

namespace Optimisation.Optimisers.EvolutionaryComputation.ParentSelection
{
    /// <summary>
    /// A parent selection operator that selects two or more parents for recombination using random number generation.
    /// </summary>
    public class ParentSelectionRandom : Operator, IParentSelectionOperator
    {
        private readonly RandomNumberManager rngManager;

        /// <summary>
        /// Creates a random parent selection operator.
        /// </summary>
        public ParentSelectionRandom() : base("Random")
        {
            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a subset of the population at random.
        /// </summary>
        /// <param name="population">The <see cref="Population"/> to be selected from.</param>
        /// <param name="numberToSelect">The number of individuals to parents to select.</param>
        /// <returns>A list of <see cref="Individual"/>s selected.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when population is smaller than the number of individuals requested.</exception>
        public IEnumerable<Individual> Select(Population population, int numberToSelect)
        {
            var selectedIdxs = rngManager.GetLocations(population.Count, numberToSelect, false, 1);
            return selectedIdxs.Select(i => population[i]);
        }
    }
}