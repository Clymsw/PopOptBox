using Optimisation.Optimisers.NelderMead.Simplices;
using System;
using System.Collections.Generic;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using System.Linq;

namespace Optimisation.Optimisers.NelderMead
{
    public class NelderMead : Optimiser
    {
        protected NelderMeadSimplexOperationsManager vertexCreator;

        #region Simplex Management

        //Logic management
        public NelderMeadSimplexOperations CurrentOperation { get; private set; } 
            = NelderMeadSimplexOperations.R;
        private Individual tempReflect;

        //History management
        public NelderMeadSteps LastStep { get; private set; }
        private readonly List<NelderMeadSimplexOperations> tempProgress =
            new List<NelderMeadSimplexOperations>();

        //Population management
        /// <summary>
        /// List of vertices from initial simplex which have not yet been reinserted
        /// </summary>
        protected readonly List<DecisionVector> initialVerticesStillUnevaluated =
            new List<DecisionVector>();
        /// <summary>
        /// List (length 0-1) of vertices which have been sent out during optimisation
        /// </summary>
        protected readonly List<DecisionVector> verticesNotEvaluated =
            new List<DecisionVector>();

        #endregion

        public NelderMead(
            Func<double[], double[]> solToScoreDelegate,
            Func<double[], double> solToFitDelegate,
            Func<double[], double> penaltyDelegate,
            DecisionVector initialLocation, double simplexCreationStepSize = 1,
            double reflectionCoefficient = 1, double expansionCoefficient = 2,
            double contractionCoefficient = 0.5, double shrinkageCoefficient = 0.5) :
            base(new Simplex(initialLocation.Vector.Count), solToScoreDelegate, solToFitDelegate, penaltyDelegate)
        {
            // Set up simplex operations
            vertexCreator = new NelderMeadSimplexOperationsManager(reflectionCoefficient,
                expansionCoefficient, contractionCoefficient, shrinkageCoefficient);

            //Set up simplex
            initialVerticesStillUnevaluated = Simplex.CreateInitialVertices(
                initialLocation,
                simplexCreationStepSize);

            //Initialise historian
            tempProgress.Add(NelderMeadSimplexOperations.R);
        }

        public override string ToString()
        {
            return "Nelder Mead, " +
                vertexCreator.ToString();
        }

        protected override bool CheckAcceptable(Individual ind)
        {
            return true;
        }

        protected override DecisionVector GetNewDecisionVector()
        {
            if (initialVerticesStillUnevaluated.Count > 0)
            {
                // Still creating the first simplex
                return initialVerticesStillUnevaluated.First();
            }
            else
            {
                //We're into the optimisation
                if (verticesNotEvaluated.Count == 0)
                {
                    //Create new individual
                    var newDv = vertexCreator.PerformOperation(
                        Population.GetMemberList().Select(m => m.DecisionVector), 
                        CurrentOperation);
                    verticesNotEvaluated.Add(newDv);
                    return newDv;
                }
                else
                {
                    //Provide the same individual back again
                    return verticesNotEvaluated.First();
                }
            }
        }

        protected override bool ReInsert(Individual ind)
        {
            if (initialVerticesStillUnevaluated.Count > 0)
            {
                if (initialVerticesStillUnevaluated.Contains(ind.DecisionVector))
                {
                    Population.AddIndividual(ind);
                    initialVerticesStillUnevaluated.Remove(ind.DecisionVector);
                    return true;
                }
                else
                {
                    throw new ArgumentException("This vertex was not expected during simplex initialisation");
                }
            }

            if (verticesNotEvaluated.Count == 0)
            {
                throw new System.ArgumentOutOfRangeException("This vertex was not expected");
            }
            if (ind.DecisionVector != verticesNotEvaluated.First())
            {
                throw new System.ArgumentOutOfRangeException("This vertex was not expected");
            }
            verticesNotEvaluated.Remove(ind.DecisionVector);

            //NM Logic
            var fitnesses = Population.GetMemberFitnesses();

            var bestFitness = fitnesses.First();
            var worstFitness = fitnesses.Last();
            double nextToWorstFitness;
            if (ind.DecisionVector.Vector.Count == 1)
                nextToWorstFitness = bestFitness;
            else
                nextToWorstFitness = fitnesses.ElementAt(fitnesses.Count() - 2);

            switch (CurrentOperation)
            {
                case (NelderMeadSimplexOperations.R):
                    if (ind.Fitness < nextToWorstFitness & ind.Fitness >= bestFitness)
                    {
                        // Reflection vertex lies inside the population, accept it.
                        Population.ReplaceWorst(ind);
                        chooseReflect();
                        reset();
                        return true;
                    }
                    else if (ind.Fitness < bestFitness)
                    {
                        // Reflection vertex is better than the best, try expansion.
                        CurrentOperation = NelderMeadSimplexOperations.E;
                        tryExpand();
                    }
                    else if (ind.Fitness < worstFitness & ind.Fitness >= nextToWorstFitness)
                    {
                        // Reflection vertex is strictly better than worst, 
                        //  but is worse than every other vertex, try contract out.
                        CurrentOperation = NelderMeadSimplexOperations.C;
                        tryContractOut();
                    }
                    else if (ind.Fitness >= worstFitness)
                    {
                        // Reflection vertex is the worst we've found, try contract in.
                        CurrentOperation = NelderMeadSimplexOperations.K;
                        tryContractIn();
                    }
                    tempReflect = ind;
                    break;

                case (NelderMeadSimplexOperations.E):
                    if (ind.Fitness < bestFitness)
                    {
                        // Expansion vertex is better than reflection vertex, accept it.
                        Population.ReplaceWorst(ind);
                        chooseExpand();
                    }
                    else
                    {
                        // Expansion vertex is worse than reflection vertex, accept reflection.
                        Population.ReplaceWorst(tempReflect);
                        chooseReflect();
                    }
                    reset();
                    return true;

                case (NelderMeadSimplexOperations.C):
                    if (ind.Fitness <= tempReflect.Fitness)
                    {
                        // Contract Outside vertex is better than reflection vertex, accept it.
                        Population.ReplaceWorst(ind);
                        chooseContractOut();
                        reset();
                        return true;
                    }
                    else
                    {
                        // Contract Outside vertex is worse than reflection vertex, shrink.
                        CurrentOperation = NelderMeadSimplexOperations.S;
                        tryShrink();
                    }
                    break;

                case (NelderMeadSimplexOperations.K):
                    if (ind.Fitness < worstFitness)
                    {
                        //Contract Inside vertex is better than the worst, accept it.
                        Population.ReplaceWorst(ind);
                        chooseContractIn();
                        reset();
                        return true;
                    }
                    else
                    {
                        // Contract Inside vertex is worst, shrink.
                        CurrentOperation = NelderMeadSimplexOperations.S;
                        tryShrink();
                    }
                    break;

                case (NelderMeadSimplexOperations.S):
                    Population.ReplaceWorst(ind);
                    chooseShrink();
                    reset();
                    return true;
            }

            return false;
        }

        private void reset()
        {
            CurrentOperation = NelderMeadSimplexOperations.R;
            tryReflect();
            tempReflect = null;
        }

        #region Historian
        #region Try
        private void tryReflect()
        {
            tempProgress.Add(NelderMeadSimplexOperations.R);
        }
        private void tryExpand()
        {
            tempProgress.Add(NelderMeadSimplexOperations.E);
        }
        private void tryContractOut()
        {
            tempProgress.Add(NelderMeadSimplexOperations.C);
        }
        private void tryContractIn()
        {
            tempProgress.Add(NelderMeadSimplexOperations.K);
        }
        private void tryShrink()
        {
            tempProgress.Add(NelderMeadSimplexOperations.S);
        }
        #endregion
        #region Choose
        private void chooseReflect()
        {
            if (CurrentOperation == NelderMeadSimplexOperations.R)
                LastStep = NelderMeadSteps.rR;
            else
                LastStep = NelderMeadSteps.reR;
            tempProgress.Clear();
        }
        private void chooseExpand()
        {
            LastStep = NelderMeadSteps.reE;
            tempProgress.Clear();
        }
        private void chooseContractOut()
        {
            LastStep = NelderMeadSteps.rcC;
            tempProgress.Clear();
        }
        private void chooseContractIn()
        {
            LastStep = NelderMeadSteps.rkK;
            tempProgress.Clear();
        }
        private void chooseShrink()
        {
            if (tempProgress[1] == NelderMeadSimplexOperations.C)
                LastStep = NelderMeadSteps.rcsS;
            else
                LastStep = NelderMeadSteps.rksS;
            tempProgress.Clear();
        }

        #endregion
        #endregion
    }
}
