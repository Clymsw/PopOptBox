using MathNet.Numerics.Random;
using Optimisation.Base.Variables;
using System.Collections.Generic;

namespace Optimisation.Optimisers.EvolutionaryComputation.Mutation
{
    internal class RandomMutationManager
    {
        private readonly RandomSource rng;

        public RandomMutationManager(RandomSource rng = null)
        {
            this.rng = rng ?? new MersenneTwister();
        }

        /// <summary>
        /// Gets a list of locations for mutation, depending on various settings
        /// </summary>
        /// <param name="vector">The <see cref="DecisionVector"/> which will be mutated by a <seealso cref="IMutationOperator"/>.</param>
        /// <param name="MaximumNumberOfMutations">The maximum number of mutations to perform.</param>
        /// <param name="MutationWithReplacement"><see langword="true"/> if the same location can be returned twice.</param>
        /// <param name="Lambda">The probability of performing a mutation at all.</param>
        /// <returns>A list of locations in the original decision vector.</returns>
        public List<int> GetMutationLocations(DecisionVector vector,
            int MaximumNumberOfMutations = 1,
            bool MutationWithReplacement = false,
            double Lambda = 0.1)
        {
            var locations = new List<int>();
            var i = 0;
            while (i < MaximumNumberOfMutations)
            {
                // See if we will perform a mutation
                var mutate = rng.NextDouble() <= Lambda;

                if (mutate)
                {
                    // See if we need to reduce the randomisation space
                    var offset = MutationWithReplacement
                        ? 0
                        : locations.Count;

                    // Generate a value 
                    var location = rng.Next(0, vector.Vector.Count - offset);

                    if (MutationWithReplacement)
                    {
                        // We are generating mutations with replacement - just add to list.
                        locations.Add(location);
                    }
                    else
                    {
                        // FInd the true value which the truncated space refers to
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
