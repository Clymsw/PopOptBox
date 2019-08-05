using System;
using System.Collections.Generic;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Management
{
    /// <summary>
    /// The Optimiser handles the logic for deciding which individuals to try.
    /// As a metaheuristic, it only knows about the Decision Vector and the Fitness of each Individual.
    /// </summary>
    public abstract class Optimiser : IOptimiser
    {
        #region Constructor

        /// <summary>
        /// Constructs the optimiser.
        /// </summary>
        /// <param name="initialPopulation">An initial population (can be empty).</param>
        /// <param name="solutionToFitness">Conversion function to change solution vector into fitness. <seealso cref="Individual.SetFitness(Func{double[], double})"/></param>
        /// <param name="penalty">Function determining what penalty to assign for illegal individuals. <seealso cref="Individual.SetFitness(Func{double[], double})"/></param>
        protected Optimiser(
            Population initialPopulation,
            Func<double[], double> solutionToFitness,
            Func<double[], double> penalty)
        {
            Population = initialPopulation;
            this.solutionToFitness = solutionToFitness;
            this.penalty = penalty;
        }

        #endregion

        #region Fields

        /// <summary>
        /// The current population in the optimiser
        /// </summary>
        public Population Population { get; }

        private readonly Func<double[], double> solutionToFitness;
        private readonly Func<double[], double> penalty;

        #endregion

        #region Activity

        /// <summary>
        ///     Optimiser-specific logic to implement, which works out what to try next.
        /// </summary>
        /// <returns>
        ///     The most useful next Decision Vector to evaluate. 
        ///     Return <see cref="DecisionVector.CreateForEmpty"/> to terminate <see cref="Runtime.OptimiserRunnerBasic"/>.
        /// </returns>
        protected abstract DecisionVector GetNewDecisionVector();

        /// <summary>
        /// Gets (up to) a certain number of individuals for evaluation.
        /// </summary>
        /// <param name="numDesired">Number of individuals wished for.</param>
        /// <returns>List of <see cref="Individual"/> objects.</returns>
        /// <exception cref="TimeoutException">Thrown if not enough individuals can be created.</exception>
        public IReadOnlyList<Individual> GetNextToEvaluate(int numDesired)
        {
            var listOfInds = new List<Individual>();
            var i = 0;
            while (listOfInds.Count < numDesired)
            {
                try
                {
                    var newDv = GetNewDecisionVector();
                    var newInd = new Individual(newDv);
                    newInd.SetProperty(OptimiserPropertyNames.CreationTime, DateTime.Now);
                    listOfInds.Add(newInd);
                }
                catch { }

                i++;

                if (i > numDesired + 20)
                    //Allow max 20 retries
                    throw new TimeoutException("Could not create enough acceptable new individuals");
            }

            return listOfInds;
        }

        /// <summary>
        /// Try to reinsert Individual back into the Population.
        /// </summary>
        /// <param name="individual">The individual to try to reinsert.</param>
        /// <returns>
        /// <see langword="true" /> if the individual was actually inserted;
        /// <see langword="false" /> if rejected. A <seealso cref="OptimiserPropertyNames.ReinsertionError"/> property is added to the <see cref="Individual"/>.
        /// </returns>
        protected virtual bool ReInsert(Individual individual)
        {
            try
            {
                Population.AddIndividual(individual);
            }
            catch (Exception e)
            {
                individual.SetProperty(OptimiserPropertyNames.ReinsertionError, e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reinserts individuals, sets their fitness based on the functions provided in the constructor:
        ///  - if legal, solution -> fitness
        ///  - if illegal, solution -> penalty
        ///  <seealso cref="ReInsert(Individual)"/>
        /// </summary>
        /// <param name="individualList">List of <see cref="Individual"/>s to reinsert.</param>
        /// <returns>The number of individuals successfully reinserted.</returns>
        public int ReInsert(IEnumerable<Individual> individualList)
        {
            var numInserted = 0;
            foreach (var ind in individualList)
            {
                if (ind.State != IndividualState.Evaluated)
                    throw new ArgumentException("Individual is not evaluated!");
                
                //If the individual has been evaluated and is legal, 
                // assign fitness and store in population.
                //If the individual has been evaluated but is not legal, 
                // assign soft penalty and store in population.
                ind.SetFitness(ind.Legal ? solutionToFitness : penalty);

                ind.SetProperty(
                    OptimiserPropertyNames.ReinsertionTime,
                    DateTime.Now);

                var wasReInserted = ReInsert(ind);

                if (wasReInserted)
                    numInserted += 1;
            }

            return numInserted;
        }

        #endregion
    }
}