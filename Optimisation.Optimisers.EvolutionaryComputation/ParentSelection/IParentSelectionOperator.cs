using System.Collections.Generic;
using Optimisation.Base.Management;

namespace Optimisation.Optimisers.EvolutionaryComputation.ParentSelection
{
    /// <summary>
    /// Interface for all parent selection operators, that choose parents for Recombination.
    /// </summary>
    public interface IParentSelectionOperator
    {
        /// <summary>
        /// Selects a certain number of parents from a population.
        /// </summary>
        /// <param name="population">A <see cref="Population"/> of individuals.</param>
        /// <param name="numberToSelect">The number of parents required.</param>
        /// <returns>A subset of the population.</returns>
        IEnumerable<Individual> Select(Population population, int numberToSelect);
    }
}