using System.Collections.Generic;
using PopOptBox.Base.Management;
using System.Linq;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Reinsertion
{
    /// <summary>
    /// A re-insertion operator which replaces a randomly-selected individual in the population 
    /// if the candidate individual is better.
    /// Same as G3 of Deb et al. (2002) TODO: Check this is true!
    /// </summary>
    public class ReinsertionReplaceRandom : Operator, IReinsertionOperator
    {
        private readonly RandomNumberManager rngManager;

        /// <summary>
        /// Creates a replace-random re-insertion operator.
        /// </summary>
        public ReinsertionReplaceRandom() : base("Replace a randomly-selected individual if better")
        {
            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Re-inserts an individual if it is fitter than a randomly-selected champion from the population.
        /// </summary>
        /// <param name="individuals">The <see cref="Individual"/> candidates for re-insertion.</param>
        /// <param name="population">The <see cref="Population"/> to re-insert into.</param>
        /// <returns>The number of individuals re-inserted.</returns>
        public int ReInsert(IEnumerable<Individual> individuals, Population population)
        {
            var numberInserted = 0;

            foreach (var individual in individuals)
            {
                var championIdx = rngManager.GetLocations(population.Count, maximumNumberOfLocations: 1, lambda: 1);
                var champion = population[championIdx.ElementAt(0)];

                if (individual.Fitness >= champion.Fitness)
                    continue;
                
                population.ReplaceIndividual(champion, individual);
                numberInserted++;
            }

            return numberInserted;
        }
    }
}
