using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.StructuredSearch.Simplices;

namespace PopOptBox.Optimisers.StructuredSearch
{
    public class NelderMead : Optimiser
    {
        protected NelderMeadSimplexOperationsManager OperationsManager;

        #region Simplex Management

        //Logic management
        public NelderMeadSimplexOperations CurrentOperation { get; private set; } 
            = NelderMeadSimplexOperations.R;
        
        private readonly List<NelderMeadSimplexOperations> tempProgress =
            new List<NelderMeadSimplexOperations>();
        
        private Individual tempReflect;

        //History management
        public NelderMeadSteps LastStep { get; private set; }
        
        //Population management
        /// <summary>
        /// List of vertices from initial simplex which have not yet been reinserted
        /// </summary>
        protected readonly List<DecisionVector> InitialVerticesStillUnevaluated;
        /// <summary>
        /// List (length 0-1) of vertices which have been sent out during optimisation
        /// </summary>
        protected readonly List<DecisionVector> VerticesNotEvaluated =
            new List<DecisionVector>();

        #endregion

        /// <summary>
        /// Constructs a new Nelder-Mead Simplex local search optimiser
        /// </summary>
        /// <param name="solutionToFitness"><see cref="Optimiser"/></param>
        /// <param name="penalty"><see cref="Optimiser"/></param>
        /// <param name="initialLocation">Starting location for the search</param>
        /// <param name="simplexCreationStepSize">Step size for creating the initial simplex</param>
        /// <param name="reflectionCoefficient">Reflection coefficient (only change this if you know what you are doing!)</param>
        /// <param name="expansionCoefficient">Expansion coefficient (only change this if you know what you are doing!)</param>
        /// <param name="contractionCoefficient">Contraction coefficient (only change this if you know what you are doing!)</param>
        /// <param name="shrinkageCoefficient">Shrinkage coefficient (only change this if you know what you are doing!)</param>
        public NelderMead(
            Func<double[], double> solutionToFitness,
            Func<double[], double> penalty,
            DecisionVector initialLocation, double simplexCreationStepSize = 1,
            double reflectionCoefficient = 1, double expansionCoefficient = 2,
            double contractionCoefficient = 0.5, double shrinkageCoefficient = 0.5) :
            base(new Simplex(initialLocation.Count), solutionToFitness, penalty)
        {
            // Set up simplex operations
            OperationsManager = new NelderMeadSimplexOperationsManager(reflectionCoefficient,
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
            return "Nelder Mead, " + OperationsManager;
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
                    try
                    {
                        //Create new individual
                        var newDv = OperationsManager.PerformOperation((Simplex)Population, CurrentOperation);
                        VerticesNotEvaluated.Add(newDv);
                        return newDv;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // We have gone 'out of bounds' - this is it for this optimisation.
                        return DecisionVector.CreateForEmpty();
                    }
                }
                else
                {
                    //Provide the same individual back again
                    return VerticesNotEvaluated.First();
                }
            }
        }

        protected override bool ReInsert(Individual individual)
        {
            if (InitialVerticesStillUnevaluated.Count > 0)
            {
                if (InitialVerticesStillUnevaluated.Contains(individual.DecisionVector))
                {
                    Population.AddIndividual(individual);
                    InitialVerticesStillUnevaluated.Remove(individual.DecisionVector);
                    return true;
                }
                else
                {
                    throw new ArgumentException("This vertex was not expected during simplex initialisation");
                }
            }

            if (VerticesNotEvaluated.Count == 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(individual), "This vertex was not expected");
            }
            if (!individual.DecisionVector.Equals(VerticesNotEvaluated.First()))
            {
                throw new System.ArgumentOutOfRangeException(nameof(individual), "This vertex was not expected");
            }
            VerticesNotEvaluated.Remove(individual.DecisionVector);

            //NM Logic
            var fitnesses = Population.GetMemberFitnesses();

            var bestFitness = fitnesses.First();
            var worstFitness = fitnesses.Last();
            double nextToWorstFitness;
            if (individual.DecisionVector.Count == 1)
                nextToWorstFitness = bestFitness;
            else
                nextToWorstFitness = fitnesses.ElementAt(fitnesses.Count() - 2);

            switch (CurrentOperation)
            {
                case (NelderMeadSimplexOperations.R):
                    if (individual.Fitness < nextToWorstFitness & individual.Fitness >= bestFitness)
                    {
                        // Reflection vertex lies inside the population, accept it.
                        Population.ReplaceWorst(individual);
                        ChooseReflect();
                        Reset();
                        return true;
                    }
                    else if (individual.Fitness < bestFitness)
                    {
                        // Reflection vertex is better than the best, try expansion.
                        CurrentOperation = NelderMeadSimplexOperations.E;
                        TryExpand();
                    }
                    else if (individual.Fitness < worstFitness & individual.Fitness >= nextToWorstFitness)
                    {
                        // Reflection vertex is strictly better than worst, 
                        //  but is worse than every other vertex, try contract out.
                        CurrentOperation = NelderMeadSimplexOperations.C;
                        TryContractOut();
                    }
                    else if (individual.Fitness >= worstFitness)
                    {
                        // Reflection vertex is the worst we've found, try contract in.
                        CurrentOperation = NelderMeadSimplexOperations.K;
                        TryContractIn();
                    }
                    tempReflect = individual;
                    break;

                case (NelderMeadSimplexOperations.E):
                    if (individual.Fitness < tempReflect.Fitness)
                    {
                        // Expansion vertex is better than reflection vertex, accept it.
                        Population.ReplaceWorst(individual);
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
                    if (individual.Fitness <= tempReflect.Fitness)
                    {
                        // Contract Outside vertex is better than reflection vertex, accept it.
                        Population.ReplaceWorst(individual);
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
                    if (individual.Fitness < worstFitness)
                    {
                        //Contract Inside vertex is better than the worst, accept it.
                        Population.ReplaceWorst(individual);
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
                    Population.ReplaceWorst(individual);
                    ChooseShrink();
                    Reset();
                    return true;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(CurrentOperation), "This operation is not understood.");
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
            LastStep = CurrentOperation == NelderMeadSimplexOperations.R 
                ? NelderMeadSteps.rR 
                : NelderMeadSteps.reR;
            tempProgress.Clear();
        }
        private void ChooseExpand()
        {
            LastStep = NelderMeadSteps.reE;
            tempProgress.Clear();
        }
        private void ChooseContractOut()
        {
            LastStep = NelderMeadSteps.rcC;
            tempProgress.Clear();
        }
        private void ChooseContractIn()
        {
            LastStep = NelderMeadSteps.rkK;
            tempProgress.Clear();
        }
        private void ChooseShrink()
        {
            LastStep = tempProgress[1] == NelderMeadSimplexOperations.C 
                ? NelderMeadSteps.rcsS 
                : NelderMeadSteps.rksS;
            tempProgress.Clear();
        }

        #endregion
        #endregion
    }
}
