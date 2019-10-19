using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation
{
    /// <summary>
    /// A mutation operator for <see cref="VariableContinuous"/> <see cref="DecisionVector"/> elements, which can add a random integer chosen from a set provided.
    /// </summary>
    public class MutationAddRandomIntegerFromSet : Operator, IMutationOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly int[] numberSet;
        private readonly double mutationProbability;
        private readonly int maximumNumberOfMutations;

        /// <summary>
        /// Constructs a mutation operator that adds an integer, chosen randomly from a set of numbers provided, to zero or more elements in the <see cref="DecisionVector"/>.
        /// </summary>
        /// <param name="numberSet">The set of numbers to choose from.</param>
        /// <param name="mutationProbability">The probability that any mutation will occur.</param>
        /// <param name="maximumNumberOfMutations">The maximum number of times a mutation should be tried.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the input values are illegal.</exception>
        public MutationAddRandomIntegerFromSet(IEnumerable<int> numberSet, double mutationProbability, int maximumNumberOfMutations)
            : base($"Add random number from set of size {numberSet.Count()}")
        {
            var numbers = numberSet.ToArray();

            if (mutationProbability < 0 || mutationProbability > 1)
                throw new ArgumentOutOfRangeException(nameof(mutationProbability),
                    "Mutation probability must be a value between 0 and 1.");
            this.mutationProbability = mutationProbability;

            if (maximumNumberOfMutations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfMutations),
                    "Maximum number of mutations must be greater than 0.");
            this.maximumNumberOfMutations = maximumNumberOfMutations;

            this.numberSet = numbers;

            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a new Decision Vector where elements have potentially been mutated.
        /// Uses <see cref="IVariable"/> to wrap the added integer, ensuring a valid Decision Vector is always created.
        /// </summary>
        /// <param name="decisionVector">The existing Decision Vector.</param>
        /// <returns>A new Decision Vector.</returns>
        public DecisionVector Operate(DecisionVector decisionVector)
        {
            var locationsToMutate = rngManager.GetLocations(
                decisionVector.Count, maximumNumberOfMutations,
                true, mutationProbability);

            var newDv = decisionVector.Select(i => (double)i).ToArray();
            var newDs = decisionVector.GetDecisionSpace();

            foreach (var location in locationsToMutate)
            {
                var mutationLocation = rngManager.Rng.Next(0, numberSet.Length);
                newDv[location] = Convert.ToDouble(newDs.ElementAt(location).AddOrWrap(newDv[location], numberSet[mutationLocation]));
            }

            return DecisionVector.CreateFromArray(newDs, newDv);
        }
    }
}