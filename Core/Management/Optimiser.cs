using System;
using System.Collections.Generic;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Management
{
    /// <inheritdoc />
    public abstract class Optimiser : IOptimiser
    {
        #region Constructor

        /// <inheritdoc />
        protected Optimiser(
            Population initialPopulation,
            string solProp,
            Func<double[], double> scoreToFitDelegate,
            Func<double[], double> penaltyDelegate,
            Func<double[], double[]> solutionToScoreDelegate)
        {
            Population = initialPopulation;
            solutionProperty = solProp;
            scoreToFit = scoreToFitDelegate;
            solToScore = solutionToScoreDelegate;
            penalty = penaltyDelegate;
        }

        #endregion

        #region Fields

        /// <summary>
        /// The current population in the optimiser
        /// </summary>
        protected Population Population { get; }

        private readonly string solutionProperty;
        private readonly Func<double[], double[]> solToScore;
        private readonly Func<double[], double> scoreToFit;
        private readonly Func<double[], double> penalty;

        #endregion

        #region Activity

        /// <summary>
        ///     Optimiser-specific logic to implement, which works out what to try next
        /// </summary>
        /// <returns>The most useful next Decision Vector to evaluate</returns>
        protected abstract DecisionVector GetNewIndividual();

        /// <summary>
        ///     Logic to check if a new individual is legal, according to rules
        ///     based on its Decision Vector
        /// </summary>
        /// <param name="ind">Individual to check</param>
        /// <returns>True/False</returns>
        protected abstract bool CheckAcceptable(Individual ind);

        /// <inheritdoc />
        /// <exception cref="TimeoutException">Thrown if not enough individuals can be created.</exception>
        public IReadOnlyList<Individual> GetNextToEvaluate(int numDesired)
        {
            var listOfInds = new List<Individual>();
            var i = 0;
            while (listOfInds.Count < numDesired)
            {
                var newDv = GetNewIndividual();
                var newInd = new Individual(newDv);
                
                if (CheckAcceptable(newInd))
                {
                    newInd.SetProperty(
                        OptimiserDefinitions.CreationTime,
                        DateTime.Now);
                    listOfInds.Add(newInd);
                }

                i++;

                if (i > numDesired + 20)
                    //Allow max 20 retries
                    throw new TimeoutException("Could not create enough acceptable new individuals");
            }

            return listOfInds;
        }

        /// <summary>
        ///     Try to reinsert Individual back into the Population.
        ///     Return value indicates if it actually got inserted, or rejected.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if <see cref="ind" /> was actually inserted;
        ///     <see langword="false" /> if rejected.
        /// </returns>
        protected virtual bool ReInsert(Individual ind)
        {
            try
            {
                Population.AddIndividual(ind);
            }
            catch (Exception e)
            {
                ind.SetProperty(OptimiserDefinitions.ReinsertionError, e);
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public int ReInsert(IEnumerable<Individual> individualList)
        {
            var numInserted = 0;
            foreach (var ind in individualList)
            {
                if (ind.State != IndividualStates.Evaluated)
                    throw new ArgumentException("Individual is not evaluated!");
                
                ind.SetSolution(solutionProperty);

                //If the individual has been evaluated and is legal, 
                // assign fitness and store in population.
                //If the individual has been evaluated but is not legal, 
                // assign soft penalty and store in population.
                ind.SetScore(ind.Legal ? solToScore : sol => sol);
                ind.SetFitness(ind.Legal ? scoreToFit : penalty);

                ind.SetProperty(
                    OptimiserDefinitions.ReinsertionTime,
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