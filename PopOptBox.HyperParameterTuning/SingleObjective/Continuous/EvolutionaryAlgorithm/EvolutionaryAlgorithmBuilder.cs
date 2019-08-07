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
                        20,
                        false);
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            IRecombinationOperator recombinationOperator;
            switch (recombination)
            {
                case AvailableOperators.RecombinationOperator.MultiPoint:
                    recombinationOperator = new CrossoverMultiPoint(
                        1);
                    break;
                
                case AvailableOperators.RecombinationOperator.ArithmeticTwoParentWeighted:
                    recombinationOperator = new CrossoverArithmeticWeighted();
                    break;
                
                case AvailableOperators.RecombinationOperator.Sbx:
                    recombinationOperator = new CrossoverSimulatedBinary(2);
                    break;
                
                default:
                    throw new NotImplementedException();
            }
            hyps.UpdateHyperParameterValue(EvolutionaryAlgorithmHyperParameters.Number_Of_Parents, 2);

            IMutationOperator mutationOperator;
            switch (mutation)
            {
                case AvailableOperators.MutationOperators.AddRandomNumber:
                    mutationOperator = new MutationAddRandomNumber(
                        0.2, 
                        0.1, 
                        problemSpace.Count);
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
                default:
                    throw new NotImplementedException();
            }
            
            return new EvolutionaryAlgorithmBuilder(problemSpace, hyps, 
                parentSelector, recombinationOperator, mutationOperator, reinsertionOperator);
        }
        
        public override Optimiser CreateOptimiser()
        {
            return new Optimisers.EvolutionaryComputation.EvolutionaryAlgorithm(
                new Population(CreateSolutionToFitness(),
                    CreatePenalty(),
                    HyperParameters.GetHyperParameterValue<int>(EvolutionaryAlgorithmHyperParameters.Population_Size)),
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
            return SolutionToFitness.SingleObjectiveMinimise;
        }

        protected override Func<double[], double> CreatePenalty()
        {
            return Penalty.DeathPenalty;
        }
    }
}