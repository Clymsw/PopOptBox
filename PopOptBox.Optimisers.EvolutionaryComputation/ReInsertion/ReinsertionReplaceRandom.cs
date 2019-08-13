using System;
using PopOptBox.Base.Management;
using System.Linq;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Reinsertion
{
    /// <summary>
    /// A re-insertion operator which replaces a randomly-selected individual in the population 
    /// if the candidate individual is better.
    /// Similar to G3 of Deb et al. (2002)
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
        /// <param name="population">The <see cref="Population"/> to re-insert into.</param>
        /// <param name="individual">The <see cref="Individual"/> candidate for re-insertion.</param>
        /// <param name="fitnessAssessment">Delegate used by the <see cref="Population"/> for calculating fitness.</param>
        /// <returns><see langword="true"/> if re-insertion has occurred.</returns>
        public bool ReInsert(Population population, Individual individual, Action<Individual> fitnessAssessment)
        {
            var championIdx = rngManager.GetLocations(population.Count, maximumNumberOfLocations: 1, lambda: 1);
            var champion = population[championIdx.ElementAt(0)];

            fitnessAssessment(individual);
            
            if (individual.Fitness < champion.Fitness)
            {
                population.ReplaceIndividual(champion, individual, i => { });
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
