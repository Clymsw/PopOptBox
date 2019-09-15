using System;
using System.Globalization;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation
{
    /// <summary>
    /// A mutation operator for any <see cref="DecisionVector"/> elements, which can add a random integer drawn from a Uniform distribution.
    /// </summary>
    public class MutationAddRandomInteger : Operator, IMutationOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly int minimum;
        private readonly int maximum;
        private readonly bool includeZero;
        private readonly double mutationProbability;
        private readonly int maximumNumberOfMutations;
        
        /// <summary>
        /// Constructs a mutation operator that adds a random integer from a Uniform distribution to zero or more elements in the <see cref="DecisionVector"/>.
        /// </summary>
        /// <param name="minimum">The smallest integer to add.</param>
        /// <param name="maximum">The largest integer to add.</param>
        /// <param name="includeZero">Whether to allow no change.</param>
        /// <param name="mutationProbability">The probability that any mutation will occur.</param>
        /// <param name="maximumNumberOfMutations">The maximum number of times a mutation should be tried.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the input values are illegal.</exception>
        public MutationAddRandomInteger(int minimum, int maximum, bool includeZero, double mutationProbability, int maximumNumberOfMutations) 
            : base($"Add random integer between {minimum} and {maximum} " +
                   (includeZero ? "" : "(excluding zero) ") +
                   $"to up to {maximumNumberOfMutations} locations " +
                   $"with chance {mutationProbability.ToString("F2", CultureInfo.InvariantCulture)}")
        {
            if (minimum > maximum)
                throw new ArgumentOutOfRangeException(nameof(maximum), 
                    "Largest value must be greater than smallest value.");
            this.minimum = minimum;
            this.maximum = maximum;
            
            if (minimum > 0)
                includeZero = true;
            this.includeZero = includeZero;
            
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

        public DecisionVector Operate(DecisionVector decisionVector)
        {
            var locationsToMutate = rngManager.GetLocations(
                decisionVector.Count, maximumNumberOfMutations,
                true, mutationProbability);
            
            var newDv = decisionVector.Select(i => (double)i).ToArray();
            var newDs = decisionVector.GetDecisionSpace();

            foreach (var location in locationsToMutate)
            {
                var mutation = rngManager.Rng.Next(
                    minimum, includeZero ? maximum : maximum - 1);
                if (!includeZero)
                    if (mutation >= 0)
                        mutation += 1;

                newDv[location] += mutation;
            }
            
            return DecisionVector.CreateFromArray(newDs, newDv);
        }
    }
}