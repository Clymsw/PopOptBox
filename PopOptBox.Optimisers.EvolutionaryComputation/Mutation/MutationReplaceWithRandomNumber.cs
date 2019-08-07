using System;
using System.Globalization;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation
{
    /// <summary>
    /// A mutation operator for any <see cref="DecisionVector"/>, which can replace elements with
    /// random ones drawn from their <see cref="IVariable"/> implementations.
    /// </summary>
    public class MutationReplaceWithRandomNumber : Operator, IMutationOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly double mutationProbability;
        private readonly int maximumNumberOfMutations;
        
        /// <summary>
        /// Constructs a mutation operator that can replace zero or more elements in the <see cref="DecisionVector"/> with new random values.
        /// </summary>
        /// <param name="mutationProbability">The probability that any mutation will occur.</param>
        /// <param name="maximumNumberOfMutations">The maximum number of times a mutation should be tried.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the input values are illegal.</exception>
        public MutationReplaceWithRandomNumber(double mutationProbability, int maximumNumberOfMutations)
            : base($"Replace up to {maximumNumberOfMutations} elements with a random number, " + 
                   $"with chance {mutationProbability.ToString("F2", CultureInfo.InvariantCulture)}")
        {
            if (mutationProbability < 0 || mutationProbability > 1)
                throw new ArgumentOutOfRangeException(nameof(mutationProbability), 
                    "Mutation probability must be a value between 0 and 1.");
            this.mutationProbability = mutationProbability;

            if (maximumNumberOfMutations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfMutations), 
                    "Maximum number of mutations must be greater than 0.");
            this.maximumNumberOfMutations = maximumNumberOfMutations;

            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a new Decision Vector where elements have potentially been mutated.
        /// Uses <see cref="IVariable"/> to implement the random number generation, so only valid individuals can be created.
        /// </summary>
        /// <param name="decisionVector">The existing Decision Vector.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when Decision Vector is zero length.</exception>
        public DecisionVector Operate(DecisionVector decisionVector)
        {
            if (decisionVector.Count == 0)
                throw new ArgumentException("Decision Vector must not be empty.",
                    nameof(decisionVector)); 
            
            var locationsToMutate = rngManager.GetLocations(
                decisionVector.Count, maximumNumberOfMutations,
                true, mutationProbability);
            
            var newDv = new object[decisionVector.Count];
            var newDs = decisionVector.GetDecisionSpace();
            for (var i = 0; i < decisionVector.Count; i++)
            {
                // Variable may be mutated multiple times.
                var numTimesToMutate = locationsToMutate.Count(l => l == i);

                if (numTimesToMutate == 0)
                {
                    newDv[i] = decisionVector.ElementAt(i);
                }
                else
                {
                    for (var j = 0; j < numTimesToMutate; j++)
                    {
                        newDv[i] = newDs.ElementAt(i).GetNextRandom(rngManager.Rng);
                    }
                }
            }
            return DecisionVector.CreateFromArray(newDs, newDv);
        }
    }
}