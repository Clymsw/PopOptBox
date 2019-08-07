using System;
using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.EvolutionaryComputation.Mutation;
using PopOptBox.Optimisers.EvolutionaryComputation.ParentSelection;
using PopOptBox.Optimisers.EvolutionaryComputation.Recombination;
using PopOptBox.Optimisers.EvolutionaryComputation.Reinsertion;

namespace PopOptBox.Optimisers.EvolutionaryComputation
{
    /// <summary>
    /// An Evolutionary, or Genetic, Algorithm performs guided random search,
    /// based on recombining (crossing-over) groups of parents
    /// and then mutating the children.
    /// See Goldberg, 1976; Deb et al., 2002
    /// </summary>
    public class EvolutionaryAlgorithm : Optimiser
    {
        private readonly Func<DecisionVector> initialIndividualGenerator;
        private readonly IParentSelectionOperator parentSelector;
        private readonly int numberOfParents;
        private readonly IRecombinationOperator recombinationOperator;
        private readonly IMutationOperator mutationOperator;
        private readonly IReinsertionOperator reinsertionOperator;

        /// <summary>
        /// Creates an Evolutionary Algorithm.
        /// </summary>
        /// <param name="initialPopulation">The initial population (can be empty).</param>
        /// <param name="initialIndividualGenerator">Creates new decision vectors to build the first population. <seealso cref="Base.Conversion.IModel"/></param>
        /// <param name="parentSelector">The <see cref="IParentSelectionOperator"/> to use.</param>
        /// <param name="recombinationOperator">The <see cref="IRecombinationOperator"/> to use.</param>
        /// <param name="mutationOperator">The <see cref="IMutationOperator"/> to use.</param>
        /// <param name="reinsertionOperator">The <see cref="IReinsertionOperator"/> to use.</param>
        /// <param name="hyperParameters">The <see cref="HyperParameterManager"/> object with relevant settings.</param>
        public EvolutionaryAlgorithm(
            Population initialPopulation, 
            Func<DecisionVector> initialIndividualGenerator,
            IParentSelectionOperator parentSelector,
            IRecombinationOperator recombinationOperator,
            IMutationOperator mutationOperator,
            IReinsertionOperator reinsertionOperator,
            HyperParameterManager hyperParameters) 
            : base(initialPopulation)
        {
            this.initialIndividualGenerator = initialIndividualGenerator;
            this.parentSelector = parentSelector;
            numberOfParents = hyperParameters.GetHyperParameterValue<int>(EvolutionaryAlgorithmHyperParameters.Number_Of_Parents);
            this.recombinationOperator = recombinationOperator;
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
            // Select parents
            var parents = parentSelector.Select(Population, numberOfParents);
            // Recombine their decision vectors
            var child = recombinationOperator.Operate(parents.Select(p => p.DecisionVector).ToArray());
            // Mutate the child Decision Vector
            return mutationOperator.Operate(child);
        }

        protected override bool ReInsert(Individual individual)
        {
            if (Population.IsTargetSizeReached)
            {
                Population.SetFitness(individual);
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
                "Evolutionary Algorithm",
                $"population size {Population.TargetSize}",
                $"{parentSelector} parent selection using {numberOfParents} parents",
                $"{recombinationOperator} recombination",
                $"{mutationOperator} mutation",
                $"{reinsertionOperator} reinsertion");
        }
    }
}
