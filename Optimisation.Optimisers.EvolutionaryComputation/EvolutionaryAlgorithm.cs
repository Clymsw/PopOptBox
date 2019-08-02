using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using Optimisation.Optimisers.EvolutionaryComputation.ParentSelection;
using Optimisation.Optimisers.EvolutionaryComputation.Recombination;
using Optimisation.Optimisers.EvolutionaryComputation.Reinsertion;
using System;
using System.Linq;

namespace Optimisation.Optimisers.EvolutionaryComputation
{
    /// <summary>
    /// An Evoluationary Algorithm performs guided random search on real-valued search spaces, based on the locations of groups of parents.
    /// </summary>
    class EvolutionaryAlgorithm : Optimiser
    {
        private readonly Func<DecisionVector> initialIndividualGenerator;
        private readonly IParentSelectionOperator parentSelector;
        private readonly int numberOfParents;
        private readonly IMultiParentRecombinationOperator recombinationOperator;
        private readonly IReinsertionOperator reinsertionOperator;

        /// <summary>
        /// Creates an Evolutionary Algorithm. See Deb et al. 2002
        /// </summary>
        /// <param name="initialPopulation">The initial population (can be empty).</param>
        /// <param name="solutionToScore">The conversion of solution to score.</param>
        /// <param name="scoreToFitness">The conversion of score to fitness.</param>
        /// <param name="penalty">The conversion of solution to penalty if individual is illegal.</param>
        /// <param name="initialIndividualGenerator">Creates new decision vectors to build the first population. <seealso cref="Base.Conversion.IModel"/></param>
        /// <param name="parentSelector">The <see cref="IParentSelectionOperator"/> to use.</param>
        /// <param name="recombinationOperator">The <see cref="IMultiParentRecombinationOperator"/> to use.</param>
        /// <param name="reinsertionOperator">The <see cref="IReinsertionOperator"/> to use.</param>
        public EvolutionaryAlgorithm(
            Population initialPopulation, 
            Func<double[], double[]> solutionToScore, 
            Func<double[], double> scoreToFitness, 
            Func<double[], double> penalty,
            Func<DecisionVector> initialIndividualGenerator,
            IParentSelectionOperator parentSelector,
            IMultiParentRecombinationOperator recombinationOperator,
            IReinsertionOperator reinsertionOperator,
            int numberOfParents = 5) 
            : base(initialPopulation, solutionToScore, scoreToFitness, penalty)
        {
            this.initialIndividualGenerator = initialIndividualGenerator;
            this.parentSelector = parentSelector;
            this.numberOfParents = numberOfParents;
            this.recombinationOperator = recombinationOperator;
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
            var parents = parentSelector.Select(Population, numberOfParents);
            // Cross-over their decision vectors
            return recombinationOperator.Operate(parents.Select(p => p.DecisionVector));
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
                "Evolutionary Algorithm",
                $"population {Population.TargetSize}",
                $"{parentSelector} parent selection",
                $"{recombinationOperator} recombination",
                $"{reinsertionOperator} reinsertion");
        }
    }
}
