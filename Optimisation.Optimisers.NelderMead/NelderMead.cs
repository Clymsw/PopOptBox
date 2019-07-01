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
        protected NelderMeadSimplexOperationsManager VertexCreator;

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
        protected readonly List<DecisionVector> InitialVerticesStillUnevaluated =
            new List<DecisionVector>();
        /// <summary>
        /// List (length 0-1) of vertices which have been sent out during optimisation
        /// </summary>
        protected readonly List<DecisionVector> VerticesNotEvaluated =
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
            VertexCreator = new NelderMeadSimplexOperationsManager(reflectionCoefficient,
                expansionCoefficient, contractionCoefficient, shrinkageCoefficient);

            //Set up simplex
            InitialVerticesStillUnevaluated = Simplex.CreateInitialVertices(
                initialLocation,
                simplexCreationStepSize);

            //Initialise historian
            tempProgress.Add(NelderMeadSimplexOperations.R);
        }

        public override string ToString()
        {
            return "Nelder Mead, " +
                VertexCreator.ToString();
        }

        protected override bool CheckAcceptable(Individual ind)
        {
            return true;
        }

        protected override DecisionVector GetNewDecisionVector()
        {
            if (InitialVerticesStillUnevaluated.Count > 0)
            {
                // Still creating the first simplex
                return InitialVerticesStillUnevaluated.First();
            }
            else
            {
                //We're into the optimisation
                if (VerticesNotEvaluated.Count == 0)
                {
                    //Create new individual
                    var newDv = VertexCreator.PerformOperation(
                        Population.GetMemberList().Select(m => m.DecisionVector), 
                        CurrentOperation);
                    VerticesNotEvaluated.Add(newDv);
                    return newDv;
                }
                else
                {
                    //Provide the same individual back again
                    return VerticesNotEvaluated.First();
                }
            }
        }

        protected override bool ReInsert(Individual ind)
        {
            if (InitialVerticesStillUnevaluated.Count > 0)
            {
                if (InitialVerticesStillUnevaluated.Contains(ind.DecisionVector))
                {
                    Population.AddIndividual(ind);
                    InitialVerticesStillUnevaluated.Remove(ind.DecisionVector);
                    return true;
                }
                else
                {
                    throw new ArgumentException("This vertex was not expected during simplex initialisation");
                }
            }

            if (VerticesNotEvaluated.Count == 0)
            {
                throw new System.ArgumentOutOfRangeException("This vertex was not expected");
            }
            if (ind.DecisionVector != VerticesNotEvaluated.First())
            {
                throw new System.ArgumentOutOfRangeException("This vertex was not expected");
            }
            VerticesNotEvaluated.Remove(ind.DecisionVector);

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
                        ChooseReflect();
                        Reset();
                        return true;
                    }
                    else if (ind.Fitness < bestFitness)
                    {
                        // Reflection vertex is better than the best, try expansion.
                        CurrentOperation = NelderMeadSimplexOperations.E;
                        TryExpand();
                    }
                    else if (ind.Fitness < worstFitness & ind.Fitness >= nextToWorstFitness)
                    {
                        // Reflection vertex is strictly better than worst, 
                        //  but is worse than every other vertex, try contract out.
                        CurrentOperation = NelderMeadSimplexOperations.C;
                        TryContractOut();
                    }
                    else if (ind.Fitness >= worstFitness)
                    {
                        // Reflection vertex is the worst we've found, try contract in.
                        CurrentOperation = NelderMeadSimplexOperations.K;
                        TryContractIn();
                    }
                    tempReflect = ind;
                    break;

                case (NelderMeadSimplexOperations.E):
                    if (ind.Fitness < bestFitness)
                    {
                        // Expansion vertex is better than reflection vertex, accept it.
                        Population.ReplaceWorst(ind);
                        ChooseExpand();
                    }
                    else
                    {
                        // Expansion vertex is worse than reflection vertex, accept reflection.
                        Population.ReplaceWorst(tempReflect);
                        ChooseReflect();
                    }
                    Reset();
                    return true;

                case (NelderMeadSimplexOperations.C):
                    if (ind.Fitness <= tempReflect.Fitness)
                    {
                        // Contract Outside vertex is better than reflection vertex, accept it.
                        Population.ReplaceWorst(ind);
                        ChooseContractOut();
                        Reset();
                        return true;
                    }
                    else
                    {
                        // Contract Outside vertex is worse than reflection vertex, shrink.
                        CurrentOperation = NelderMeadSimplexOperations.S;
                        TryShrink();
                    }
                    break;

                case (NelderMeadSimplexOperations.K):
                    if (ind.Fitness < worstFitness)
                    {
                        //Contract Inside vertex is better than the worst, accept it.
                        Population.ReplaceWorst(ind);
                        ChooseContractIn();
                        Reset();
                        return true;
                    }
                    else
                    {
                        // Contract Inside vertex is worst, shrink.
                        CurrentOperation = NelderMeadSimplexOperations.S;
                        TryShrink();
                    }
                    break;

                case (NelderMeadSimplexOperations.S):
                    Population.ReplaceWorst(ind);
                    ChooseShrink();
                    Reset();
                    return true;
            }

            return false;
        }

        private void Reset()
        {
            CurrentOperation = NelderMeadSimplexOperations.R;
            TryReflect();
            tempReflect = null;
        }

        #region Historian
        #region Try
        private void TryReflect()
        {
            tempProgress.Add(NelderMeadSimplexOperations.R);
        }
        private void TryExpand()
        {
            tempProgress.Add(NelderMeadSimplexOperations.E);
        }
        private void TryContractOut()
        {
            tempProgress.Add(NelderMeadSimplexOperations.C);
        }
        private void TryContractIn()
        {
            tempProgress.Add(NelderMeadSimplexOperations.K);
        }
        private void TryShrink()
        {
            tempProgress.Add(NelderMeadSimplexOperations.S);
        }
        #endregion
        #region Choose
        private void ChooseReflect()
        {
            if (CurrentOperation == NelderMeadSimplexOperations.R)
                LastStep = NelderMeadSteps.RR;
            else
                LastStep = NelderMeadSteps.ReR;
            tempProgress.Clear();
        }
        private void ChooseExpand()
        {
            LastStep = NelderMeadSteps.ReE;
            tempProgress.Clear();
        }
        private void ChooseContractOut()
        {
            LastStep = NelderMeadSteps.RcC;
            tempProgress.Clear();
        }
        private void ChooseContractIn()
        {
            LastStep = NelderMeadSteps.RkK;
            tempProgress.Clear();
        }
        private void ChooseShrink()
        {
            if (tempProgress[1] == NelderMeadSimplexOperations.C)
                LastStep = NelderMeadSteps.RcsS;
            else
                LastStep = NelderMeadSteps.RksS;
            tempProgress.Clear();
        }

        #endregion
        #endregion
    }
}
