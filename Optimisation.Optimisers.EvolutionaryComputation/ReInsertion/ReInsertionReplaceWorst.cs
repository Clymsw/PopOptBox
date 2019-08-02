using Optimisation.Base.Management;

namespace Optimisation.Optimisers.EvolutionaryComputation.Reinsertion
{
    /// <summary>
    /// A re-insertion operator which replaces the worst current individual in the population if the candidate individual is better.
    /// </summary>
    public class ReInsertionReplaceWorst : Operator, IReInsertionOperator
    {
        /// <summary>
        /// Creates a replace-worst re-insertion operator.
        /// </summary>
        public ReInsertionReplaceWorst() : base("Replace worst if better")
        {
        }

        /// <summary>
        /// Re-inserts an individual if it is fitter than the worst in the population.
        /// </summary>
        /// <param name="population">The <see cref="Population"/> to re-insert into.</param>
        /// <param name="individual">The <see cref="Individual"/> candidate for re-insertion.</param>
        /// <returns><see langword="true"/> if re-insertion has occurred.</returns>
        public bool ReInsert(Population population, Individual individual)
        {
            if (individual.Fitness < population.Worst().Fitness)
            {
                population.ReplaceWorst(individual);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
