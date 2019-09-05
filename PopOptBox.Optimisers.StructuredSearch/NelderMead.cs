using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.StructuredSearch.Simplices;

namespace PopOptBox.Optimisers.StructuredSearch
{
    public class NelderMead : Optimiser
    {
        protected readonly NelderMeadSimplexOperationsManager OperationsManager;

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
        /// Constructs a new Nelder-Mead Simplex local search optimiser.
        /// </summary>
        /// <param name="fitnessCalculator">A <see cref="FitnessCalculatorSingleObjective"/>. <see cref="Optimiser"/></param>
        /// <param name="initialLocation">Starting location for the search.</param>
        /// <param name="hyperParameters">
        /// Instance of <see cref="HyperParameterManager"/> containing values for
        /// all the coefficients required by <see cref="NelderMeadHyperParameters"/>.
        /// </param>
        public NelderMead(
            IFitnessCalculator fitnessCalculator,
            DecisionVector initialLocation,
            HyperParameterManager hyperParameters) :
            base(new Simplex(initialLocation.Count), fitnessCalculator)
        {
            // Set up simplex operations
            OperationsManager = new NelderMeadSimplexOperationsManager(hyperParameters);

            //Set up simplex
            InitialVerticesStillUnevaluated = Simplex.CreateInitialVertices(
                initialLocation,
                hyperParameters.GetHyperParameterValue<double>(NelderMeadHyperParameters.Simplex_Creation_Step_Size));

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
                        var newDv = OperationsManager.PerformOperation((Simplex) Population, CurrentOperation);
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

        protected override int AssessFitnessAndDecideFate(IEnumerable<Individual> individuals)
        {
            // Assign fitness
            var inds = individuals as Individual[] ?? individuals.ToArray();
            fitnessCalculator.CalculateAndAssignFitness(inds, null);

            var numberInserted = 0;
            foreach (var individual in inds)
            {
                // Initial simplex creation
                if (InitialVerticesStillUnevaluated.Count > 0)
                {
                    if (InitialVerticesStillUnevaluated.Contains(individual.DecisionVector))
                    {
                        Population.AddIndividual(individual);
                        InitialVerticesStillUnevaluated.Remove(individual.DecisionVector);
                        numberInserted++;
                        continue;
                    }
                    else
                    {
                        individual.SetProperty(OptimiserPropertyNames.ReinsertionError,
                            "This vertex was not expected during simplex initialisation");
                        continue;
                    }
                }

                if (VerticesNotEvaluated.Count == 0)
                {
                    individual.SetProperty(OptimiserPropertyNames.ReinsertionError,
                        "This vertex was not expected during simplex initialisation");
                    continue;
                }

                if (!individual.DecisionVector.Equals(VerticesNotEvaluated.First()))
                {
                    individual.SetProperty(OptimiserPropertyNames.ReinsertionError,
                        "This vertex was not expected during simplex initialisation");
                    continue;
                }

                VerticesNotEvaluated.Remove(individual.DecisionVector);

                // Into Nelder Mead logic
                var fitnesses = Population.GetMemberFitnesses().ToArray();

                var bestFitness = fitnesses.First();
                var worstFitness = fitnesses.Last();
                double nextToWorstFitness;
                nextToWorstFitness = individual.DecisionVector.Count == 1 
                    ? bestFitness 
                    : fitnesses.ElementAt(fitnesses.Count() - 2);

                switch (CurrentOperation)
                {
                    case (NelderMeadSimplexOperations.R):
                        if (individual.Fitness < nextToWorstFitness & individual.Fitness >= bestFitness)
                        {
                            // Reflection vertex lies inside the population, accept it.
                            Population.ReplaceWorst(individual);
                            ChooseReflect();
                            Reset();
                            numberInserted++;
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
                        numberInserted++;
                        break;

                    case (NelderMeadSimplexOperations.C):
                        if (individual.Fitness <= tempReflect.Fitness)
                        {
                            // Contract Outside vertex is better than reflection vertex, accept it.
                            Population.ReplaceWorst(individual);
                            ChooseContractOut();
                            Reset();
                            numberInserted++;
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
                            numberInserted++;
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
                        numberInserted++;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(CurrentOperation),
                            "This operation is not understood.");
                }
            }

            return numberInserted;
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