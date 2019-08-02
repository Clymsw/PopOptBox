using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using Optimisation.Optimisers.EvolutionaryComputation.Mutation;
using Optimisation.Optimisers.EvolutionaryComputation.ParentSelection;
using Optimisation.Optimisers.EvolutionaryComputation.Recombination;
using Optimisation.Optimisers.EvolutionaryComputation.Reinsertion;
using System;
using System.Linq;

namespace Optimisation.Optimisers.EvolutionaryComputation
{
    /// <summary>
    /// A Genetic Algorithm performs guided random search using the breeding and mutation of individual pairs, similar to animal reproduction.
    /// </summary>
    class GeneticAlgorithm : Optimiser
    {
        private readonly Func<DecisionVector> initialIndividualGenerator;
        private readonly IParentSelectionOperator parentSelector;
        private readonly ITwoParentCrossoverOperator crossoverOperator;
        private readonly IMutationOperator mutationOperator;
        private readonly IReinsertionOperator reinsertionOperator;

        /// <summary>
        /// Creates a Genetic Algorithm.
        /// </summary>
        /// <param name="initialPopulation">The initial population (can be empty).</param>
        /// <param name="solutionToScore">The conversion of solution to score.</param>
        /// <param name="scoreToFitness">The conversion of score to fitness.</param>
        /// <param name="penalty">The conversion of solution to penalty if individual is illegal.</param>
        /// <param name="initialIndividualGenerator">Creates new decision vectors to build the first population. <seealso cref="Base.Conversion.IModel"/></param>
        /// <param name="parentSelector">The <see cref="IParentSelectionOperator"/> to use.</param>
        /// <param name="crossoverOperator">The <see cref="ITwoParentCrossoverOperator"/> to use.</param>
        /// <param name="mutationOperator">The <see cref="IMutationOperator"/> to use.</param>
        /// <param name="reinsertionOperator">The <see cref="IReinsertionOperator"/> to use.</param>
        public GeneticAlgorithm(
            Population initialPopulation, 
            Func<double[], double[]> solutionToScore, 
            Func<double[], double> scoreToFitness, 
            Func<double[], double> penalty,
            Func<DecisionVector> initialIndividualGenerator,
            IParentSelectionOperator parentSelector,
            ITwoParentCrossoverOperator crossoverOperator,
            IMutationOperator mutationOperator,
            IReinsertionOperator reinsertionOperator) 
            : base(initialPopulation, solutionToScore, scoreToFitness, penalty)
        {
            this.initialIndividualGenerator = initialIndividualGenerator;
            this.parentSelector = parentSelector;
            this.crossoverOperator = crossoverOperator;
            this.mutationOperator = mutationOperator;
            this.reinsertionOperator = reinsertionOperator;
        }

        protected override DecisionVector GetNewDecisionVector()
        {
            return Population.IsTargetSizeReached
                ? evolveNewIndividual()
                : initialIndividualGenerator();
        }

        private DecisionVector evolveNewIndividual()
        {
            // Select two parents
            var parents = parentSelector.Select(Population, 2);
            // Cross-over their decision vectors
            var child = crossoverOperator.Operate(parents.ElementAt(0).DecisionVector, parents.ElementAt(1).DecisionVector);
            // Mutate the child decision vector
            return mutationOperator.Operate(child);
        }

        protected override bool ReInsert(Individual individual)
        {
            if (Population.IsTargetSizeReached)
            {
                return reinsertionOperator.ReInsert(Population, individual);
            }
            else
            {
                return base.ReInsert(individual);
            }
        }

        public override string ToString()
        {
            return string.Join(", with ",
                "Genetic Algorithm",
                $"population {Population.TargetSize}",
                $"{parentSelector} parent selection",
                $"{crossoverOperator} crossover",
                $"{mutationOperator} mutation",
                $"{reinsertionOperator} reinsertion");
        }
    }
}
