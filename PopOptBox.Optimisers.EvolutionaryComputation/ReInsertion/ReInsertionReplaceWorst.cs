using System.Collections.Generic;
using PopOptBox.Base.Management;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Reinsertion
{
    /// <summary>
    /// A re-insertion operator which replaces the worst current individual in the population 
    /// if the candidate individual is better.
    /// </summary>
    public class ReinsertionReplaceWorst : Operator, IReinsertionOperator
    {
        /// <summary>
        /// Creates a replace-worst re-insertion operator.
        /// </summary>
        public ReinsertionReplaceWorst() : base("Replace worst if better")
        {
        }

        /// <summary>
        /// Re-inserts an individual if it is fitter than the worst in the population.
        /// </summary>
        /// <param name="individuals">The <see cref="Individual"/> candidates for re-insertion.</param>
        /// <param name="population">The <see cref="Population"/> to re-insert into.</param>
        /// <returns>The number of individuals re-inserted.</returns>
        public int ReInsert(IEnumerable<Individual> individuals, Population population)
        {
            var numberInserted = 0;
            
            foreach (var individual in individuals)
            {
                if (individual.Fitness >= population.Worst().Fitness) 
                    continue;
                
                population.ReplaceWorst(individual);
                numberInserted++;
            }

            return numberInserted;
        }
    }
}
