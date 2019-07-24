using System;
using MathNet.Numerics.Random;
using Optimisation.Base.Variables;
using System.Collections.Generic;

namespace Optimisation.Optimisers.EvolutionaryComputation.Mutation
{
    public class RandomMutationManager
    {
        public readonly RandomSource rng;

        public RandomMutationManager(RandomSource rng = null)
        {
            this.rng = rng ?? new MersenneTwister(threadSafe: true);
        }

        /// <summary>
        /// Gets a list of locations for mutation, depending on various settings
        /// </summary>
        /// <param name="vector">The <see cref="DecisionVector"/> which will be mutated by a <seealso cref="IMutationOperator"/>.</param>
        /// <param name="maximumNumberOfMutations">The maximum number of mutations to perform.</param>
        /// <param name="mutationWithReplacement"><see langword="true"/> if the same location can be returned twice.</param>
        /// <param name="lambda">The probability of performing a mutation at all.</param>
        /// <returns>A list of locations in the original decision vector.</returns>
        public IEnumerable<int> GetMutationLocations(DecisionVector vector,
            int maximumNumberOfMutations = 1,
            bool mutationWithReplacement = false,
            double lambda = 0.1)
        {
            if (maximumNumberOfMutations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfMutations),
                    "Maximum number of mutations must be greater than 0.");
            
            if (!mutationWithReplacement && maximumNumberOfMutations > vector.Vector.Count)
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfMutations),
                    "If sampling without replacement, cannot ask for more mutation locations than are available.");
            
            if (lambda < 0 || lambda > 1)
                throw  new ArgumentOutOfRangeException(nameof(lambda),
                    "The probability of a mutation must be between 0 and 1.");
            
            var locations = new List<int>();
            var i = 0;
            while (i < maximumNumberOfMutations)
            {
                // See if we will perform a mutation
                var mutate = rng.NextDouble() <= lambda;

                if (mutate)
                {
                    // See if we need to reduce the randomisation space
                    var offset = mutationWithReplacement
                        ? 0
                        : locations.Count;

                    // Generate a value 
                    var location = rng.Next(0, vector.Vector.Count - offset);

                    if (mutationWithReplacement)
                    {
                        // We are generating mutations with replacement - just add to list.
                        locations.Add(location);
                    }
                    else
                    {
                        // Find the true value which the truncated space refers to
                        while (locations.Contains(location))
                            location++;
                        // Add to list
                        locations.Add(location);
                    }
                }
                i++;
            }
            return locations;
        }
    }
}
