using System;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.FitnessCalculation;
using PopOptBox.Base.Management;
using PopOptBox.Base.MultiObjectiveCalculation;
using PopOptBox.Base.Variables;
using PopOptBox.HyperParameterTuning.Helpers.Continuous;
using PopOptBox.HyperParameterTuning.SingleObjective.Continuous.EvolutionaryAlgorithm;
using PopOptBox.Optimisers.EvolutionaryComputation;
using PopOptBox.Optimisers.EvolutionaryComputation.MultiObjective;
using PopOptBox.Optimisers.EvolutionaryComputation.Mutation;
using PopOptBox.Optimisers.EvolutionaryComputation.ParentSelection;
using PopOptBox.Optimisers.EvolutionaryComputation.Recombination;
using PopOptBox.Optimisers.EvolutionaryComputation.Reinsertion;

namespace PopOptBox.HyperParameterTuning.MultipleObjective.Continuous.EvolutionaryAlgorithm
{
    public class EvolutionaryAlgorithmBuilderContinuousMO : OptimiserBuilder
    {
        private readonly Population population;
        private readonly DecisionSpace decisionSpace;
        private readonly IParentSelectionOperator parentSelector;
        private readonly IRecombinationOperator recombinationOperator;
        private readonly IMutationOperator mutationOperator;
        private readonly IReinsertionOperator reinsertionOperator;
        
        public EvolutionaryAlgorithmBuilderContinuousMO(
            Population population,
            DecisionSpace decisionSpace, 
            HyperParameterManager hyperParameters,
            IParentSelectionOperator parentSelector,
            IRecombinationOperator recombinationOperator,
            IMutationOperator mutationOperator,
            IReinsertionOperator reinsertionOperator)
        {
            this.decisionSpace = decisionSpace;
            HyperParameters.AddFromExistingHyperParameterSet(hyperParameters);
            this.population = population;
            this.parentSelector = parentSelector;
            this.recombinationOperator = recombinationOperator;
            this.mutationOperator = mutationOperator;
            this.reinsertionOperator = reinsertionOperator;
        }
        
        public static OptimiserBuilder GetBuilder(DecisionSpace problemSpace)
        {
            var hyps = EvolutionaryAlgorithmHyperParameters.GetDefaultHyperParameters();
            
            var population = new Population(
                200);
            hyps.UpdateHyperParameterValue(
                EvolutionaryAlgorithmHyperParameters.Population_Size, 
                200);
                        
            IParentSelectionOperator parentSelector = new ParentSelectionTournament(
                20, false);
            
            IRecombinationOperator recombinationOperator = new CrossoverSimulatedBinary(
                2);; 
            hyps.UpdateHyperParameterValue(
                EvolutionaryAlgorithmHyperParameters.Number_Of_Parents, 2);

            IMutationOperator mutationOperator = new MutationAddRandomNumber(
                0.1, 
                0.1, 
                1);
            
            IReinsertionOperator reinsertionOperator = new ReinsertionReplaceRandom();

            return new EvolutionaryAlgorithmBuilderContinuousMO(population, problemSpace, hyps, 
                parentSelector, recombinationOperator, mutationOperator, reinsertionOperator);
        }
        
        public override Optimiser CreateOptimiser()
        {
            population.Clear();
            
            return new Optimisers.EvolutionaryComputation.EvolutionaryAlgorithm(
                population,
                CreateFitnessCalculator(),
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

        protected override IFitnessCalculator CreateFitnessCalculator()
        {
            return new Nsga2(
                new[] {true, true},
                population.TargetSize,
                new FastNonDominatedSort(),
                true);
        }
    }
}