using System;
using System.Collections.Generic;
using PopOptBox.Base.Management;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Reinsertion
{
    /// <summary>
    /// Interface for all re-insertion operators, that is called by <see cref="EvolutionaryAlgorithm.AssessFitnessAndDecideFate"/>.
    /// </summary>
    public interface IReinsertionOperator
    {
        /// <summary>
        /// Perform the re-insertion.
        /// </summary>
        /// <param name="individuals">The <see cref="Individual"/> candidates for re-insertion.</param>
        /// <param name="population">The <see cref="Population"/> to re-insert into.</param>
        /// <returns>The number of individuals re-inserted.</returns>
        int ReInsert(IEnumerable<Individual> individuals, Population population);
    }
}
