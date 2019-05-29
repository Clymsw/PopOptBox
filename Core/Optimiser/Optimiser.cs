using System;
using System.Collections.Generic;
using Optimisation.Core.Population;

namespace Optimisation.Core.Optimiser
{
    /// <inheritdoc />
    public abstract class Optimiser<TDecVec> : IOptimiser<TDecVec>
    {
        #region Constructor

        /// <inheritdoc />
        protected Optimiser(string solProp,
            Func<double[], double> scoreToFitDelegate,
            Func<double[], double> penaltyDelegate,
            Func<double[], double[]> solutionToScoreDelegate)
        {
            solutionProperty = solProp;
            scoreToFit = scoreToFitDelegate;
            solToScore = solutionToScoreDelegate;
            penalty = penaltyDelegate;
        }

        #endregion

        #region Fields

        protected Population<TDecVec> population;

        /// <summary>
        ///     The current population in the optimiser
        /// </summary>
        public Population<TDecVec> Population => population;

        private readonly string solutionProperty;
        private readonly Func<double[], double[]> solToScore;
        private readonly Func<double[], double> scoreToFit;
        private readonly Func<double[], double> penalty;

        #endregion

        #region Activity

        /// <summary>
        ///     Optimiser-specific logic to implement, which works out what to try next
        /// </summary>
        /// <returns>The most useful next Individual to evaluate</returns>
        protected abstract Individual<TDecVec> GetNextInd();

        /// <summary>
        ///     Logic to check if a new individual is legal, according to rules
        ///     based on its Decision Vector
        /// </summary>
        /// <param name="ind">Individual to check</param>
        /// <returns>True/False</returns>
        protected abstract bool CheckAcceptable(Individual<TDecVec> ind);

        /// <inheritdoc />
        /// <exception cref="TimeoutException">Thrown if not enough individuals can be created.</exception>
        public IReadOnlyList<Individual<TDecVec>> GetNextToEvaluate(int numDesired)
        {
            var listOfInds = new List<Individual<TDecVec>>();
            var i = 0;
            while (listOfInds.Count < numDesired)
            {
                var newInd = GetNextInd();
                if (CheckAcceptable(newInd))
                {
                    newInd.SendForEvaluation();
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
        protected abstract bool ReInsert(Individual<TDecVec> ind);

        /// <inheritdoc />
        public int ReInsert(IEnumerable<Individual<TDecVec>> individualList)
        {
            var numInserted = 0;
            foreach (var ind in individualList)
            {
                if (ind.State != IndividualStates.Evaluated)
                    //TODO: Use logging function instead
                    throw new InvalidOperationException("Individual not yet evaluated");

                ind.SetSolution(solutionProperty);

                //If the individual has been evaluated and is legal, 
                // assign fitness and store in population.
                //If the individual has been evaluated but is not legal, 
                // assign soft penalty and store in population.
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