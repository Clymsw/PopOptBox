using System;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.EvolutionaryComputation;
using PopOptBox.Optimisers.EvolutionaryComputation.Mutation;
using PopOptBox.Optimisers.EvolutionaryComputation.ParentSelection;
using PopOptBox.Optimisers.EvolutionaryComputation.Recombination;
using PopOptBox.Optimisers.EvolutionaryComputation.Reinsertion;

namespace PopOptBox.HyperParameterTuning.SingleObjective.Continuous.EvolutionaryAlgorithm
{
    public class EvolutionaryAlgorithmBuilder : OptimiserBuilder
    {
        private readonly DecisionSpace decisionSpace;
        private readonly IParentSelectionOperator parentSelector;
        private readonly IRecombinationOperator recombinationOperator;
        private readonly IMutationOperator mutationOperator;
        private readonly IReinsertionOperator reinsertionOperator;
            
        public EvolutionaryAlgorithmBuilder(
            DecisionSpace decisionSpace, 
            HyperParameterManager hyperParameters,
            IParentSelectionOperator parentSelector,
            IRecombinationOperator recombinationOperator,
            IMutationOperator mutationOperator,
            IReinsertionOperator reinsertionOperator)
        {
            this.decisionSpace = decisionSpace;
            HyperParameters.AddFromExistingHyperParameterSet(hyperParameters);

            this.parentSelector = parentSelector;
            this.recombinationOperator = recombinationOperator;
            this.mutationOperator = mutationOperator;
            this.reinsertionOperator = reinsertionOperator;
        }
        
        public static OptimiserBuilder GetBuilder(DecisionSpace problemSpace,
            AvailableOperators.ParentSelector parentSelection,
            AvailableOperators.RecombinationOperator recombination,
            AvailableOperators.MutationOperators mutation,
            AvailableOperators.ReinsertionOperators reinsertion,
            int? populationSize = null)
        {
            var hyps = EvolutionaryAlgorithmHyperParameters.GetDefaultHyperParameters();
            
            if (populationSize != null)
                hyps.UpdateHyperParameterValue(
                    EvolutionaryAlgorithmHyperParameters.Population_Size, populationSize);

            IParentSelectionOperator parentSelector;
            switch (parentSelection)
            {
                case AvailableOperators.ParentSelector.Tournament:
                    parentSelector = new ParentSelectionTournament(
                        40,
                        false);
                    break;
                
                case AvailableOperators.ParentSelector.Greedy:
                    parentSelector = new ParentSelectionGreedy();
                    break;
                
                case AvailableOperators.ParentSelector.Random:
                    parentSelector = new ParentSelectionRandom();
                    break;
                
                case AvailableOperators.ParentSelector.Roulette:
                    parentSelector = new ParentSelectionRoulette(
                        false);
                    break;
                
                default:
                    throw new NotImplementedException();
            }
            
            IRecombinationOperator recombinationOperator;
            hyps.UpdateHyperParameterValue(EvolutionaryAlgorithmHyperParameters.Number_Of_Parents, 2);
            switch (recombination)
            {
                case AvailableOperators.RecombinationOperator.MultiPoint:
                    recombinationOperator = new CrossoverMultiPoint(
                        1);
                    break;
                
                case AvailableOperators.RecombinationOperator.ArithmeticTwoParentWeighted:
                    recombinationOperator = new CrossoverArithmeticWeighted(
                        false,
                        0.5);
                    break;
                
                case AvailableOperators.RecombinationOperator.Sbx:
                    recombinationOperator = new CrossoverSimulatedBinary(
                        2);
                    break;

                case AvailableOperators.RecombinationOperator.ArithmeticMultiParent:
                    recombinationOperator = new CrossoverArithmeticMultiParent();
                    hyps.UpdateHyperParameterValue(
                        EvolutionaryAlgorithmHyperParameters.Number_Of_Parents, 
                        4);
                    break;
                
                case AvailableOperators.RecombinationOperator.Uniform:
                    recombinationOperator = new CrossoverUniform(
                        0.5);
                    break;
                
                case AvailableOperators.RecombinationOperator.Pcx:
                    recombinationOperator = new RecombinationParentCentric(
                        0.1, 
                        0.1);
                    hyps.UpdateHyperParameterValue(
                        EvolutionaryAlgorithmHyperParameters.Number_Of_Parents, 
                        6);
                    break;
                
                default:
                    throw new NotImplementedException();
            }

            IMutationOperator mutationOperator;
            switch (mutation)
            {
                case AvailableOperators.MutationOperators.AddRandomNumber:
                    mutationOperator = new MutationAddRandomNumber(
                        0.1, 
                        0.1, 
                        1);
                    break;
                
                case AvailableOperators.MutationOperators.RandomSwap:
                    mutationOperator = new MutationRandomSwap(
                        0.1);
                    break;
                
                case AvailableOperators.MutationOperators.ReplaceWithRandomNumber:
                    mutationOperator = new MutationReplaceWithRandomNumber(
                        0.1, 
                        1);
                    break;

                case AvailableOperators.MutationOperators.None:
                    mutationOperator = new MutationReplaceWithRandomNumber(
                        0,1);
                    break;
                
                default:
                    throw new NotImplementedException();
            }

            IReinsertionOperator reinsertionOperator;
            switch (reinsertion)
            {
                case AvailableOperators.ReinsertionOperators.ReplaceWorst:
                    reinsertionOperator = new ReinsertionReplaceWorst();
                    break;
                
                case AvailableOperators.ReinsertionOperators.ReplaceRandom:
                    reinsertionOperator = new ReinsertionReplaceRandom();
                    break;
                
                default:
                    throw new NotImplementedException();
            }
            
            return new EvolutionaryAlgorithmBuilder(problemSpace, hyps, 
                parentSelector, recombinationOperator, mutationOperator, reinsertionOperator);
        }
        
        public override Optimiser CreateOptimiser()
        {
            return new Optimisers.EvolutionaryComputation.EvolutionaryAlgorithm(
                new Population(HyperParameters.GetHyperParameterValue<int>(EvolutionaryAlgorithmHyperParameters.Population_Size)),
                CreateSolutionToFitness(),
                CreatePenalty(),
                CreateDecisionVectorGenerator(),
                parentSelector,
                recombinationOperator,
                mutationOperator,
                reinsertionOperator,
                HyperParameters);
        }

        public override IModel CreateModel()
        {
            return new ContinuousProblemModel(decisionSpace);
        }

        protected override Func<double[], double> CreateSolutionToFitness()
        {
            return SolutionToFitnessSingleObjective.Minimise;
        }

        protected override Func<double[], double> CreatePenalty()
        {
            return Penalty.DeathPenalty;
        }
    }
}