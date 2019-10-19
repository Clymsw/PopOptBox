using System;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation
{
    /// <summary>
    /// A super-mutation operator that only applies a given mutation to elements of a <see cref="DecisionVector"/> if the provided list of conditions are met.
    /// </summary>
    public class MutationConditional : IMutationOperator
    {
        private readonly IMutationOperator mutation;
        private readonly Func<object, bool>[] conditions;

        /// <summary>
        /// Constructs a mutation operator that restricts application of a mutation to elements of a <see cref="DecisionVector"/> where all conditions are met.
        /// </summary>
        /// <param name="mutation">The <see cref="IMutationOperator"/> to restrict.</param>
        /// <param name="conditions">The restrictions to apply on the current value of each <see cref="DecisionVector"/> element.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when conditions are null or empty.</exception>
        public MutationConditional(IMutationOperator mutation, params Func<object, bool>[] conditions)
        {
            this.mutation = mutation;

            if (conditions == null || conditions.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(conditions), "The conditions cannot be null or empty.");

            this.conditions = conditions;
        }

        /// <summary>
        /// Gets a new Decision Vector where elements which pass the conditions have potentially been mutated.
        /// </summary>
        /// <param name="decisionVector"><see cref="DecisionVector"/> to mutate.</param>
        /// <returns>A mutated Decision Vector.</returns>
        public DecisionVector Operate(DecisionVector decisionVector)
        {
            var elementsToMutate = decisionVector.Select(e => conditions.All(c => c(e)));
            var tempDs = new DecisionSpace(decisionVector.GetDecisionSpace().Where((d, i) => elementsToMutate.ElementAt(i)));
            var tempDv = DecisionVector.CreateFromArray(tempDs, decisionVector.Where((e, i) => elementsToMutate.ElementAt(i)));
            var newElements = mutation.Operate(tempDv);

            var newVector = decisionVector.ToArray();
            var item = 0;
            for (var i = 0; i < newVector.Length; i++)
            {
                if (elementsToMutate.ElementAt(i))
                {
                    newVector[i] = newElements.ElementAt(item);
                    item++;
                }
            }

            return DecisionVector.CreateFromArray(decisionVector.GetDecisionSpace(), newVector);
        }
    }
}