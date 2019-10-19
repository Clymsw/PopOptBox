using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation
{
    /// <summary>
    /// A mutation operator aggregator, which performs several mutations in order.
    /// </summary>
    public class MutationCombination : IMutationOperator
    {
        private readonly IMutationOperator[] mutations;

        /// <summary>
        /// Constructs a mutation operator that aggregates multiple mutations.
        /// </summary>
        /// <param name="mutations">The <see cref="IMutationOperator"/>s to aggregate.</param>
        public MutationCombination(params IMutationOperator[] mutations)
        {
            this.mutations = mutations;
        }
        
        /// <summary>
        /// Perform mutations in the order they were passed into the constructor.
        /// </summary>
        /// <param name="decisionVector"><see cref="DecisionVector"/> to mutate.</param>
        /// <returns>A mutated Decision Vector.</returns>
        public DecisionVector Operate(DecisionVector decisionVector)
        {
            return mutations.Aggregate(decisionVector, (current, mutation) => mutation.Operate(current));
        }
    }
}