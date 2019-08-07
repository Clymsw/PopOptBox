using System;
using MathNet.Numerics.Random;
using PopOptBox.Base.Variables;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;

namespace PopOptBox.Optimisers.EvolutionaryComputation
{
    internal class RandomNumberManager
    {
        public readonly RandomSource Rng;

        /// <summary>
        /// Constructs the random number manager.
        /// Tries to ensure thread-safe RNG.
        /// </summary>
        /// <param name="rng">A random number generator.</param>
        public RandomNumberManager(RandomSource rng = null)
        {
            this.Rng = rng ?? new MersenneTwister(threadSafe: true);
        }
        
        /// <summary>
        /// Gets a list of random indices into an array, depending on various settings.
        /// </summary>
        /// <param name="numberOfLocationsToChooseFrom">The array length, indexes to which will be returned.</param>
        /// <param name="maximumNumberOfLocations">The maximum number of locations to select.</param>
        /// <param name="selectionWithReplacement"><see langword="true"/> if the same location can be returned more than once.</param>
        /// <param name="lambda">The probability of choosing a location at all.</param>
        /// <returns>A list of locations in the original array.</returns>
        public IEnumerable<int> GetLocations(
            int numberOfLocationsToChooseFrom,
            int maximumNumberOfLocations = 1,
            bool selectionWithReplacement = false,
            double lambda = 0.1)
        {
            if (maximumNumberOfLocations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfLocations),
                    "Maximum number of locations must be greater than 0.");
            
            if (!selectionWithReplacement && maximumNumberOfLocations > numberOfLocationsToChooseFrom)
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfLocations),
                    "If sampling without replacement, cannot ask for more locations than are available.");
            
            if (lambda < 0 || lambda > 1)
                throw  new ArgumentOutOfRangeException(nameof(lambda),
                    "The probability of selecting a location must be between 0 and 1.");
            
            if (lambda == 1 && !selectionWithReplacement)
            {
                // There's a fast function implemented for this...
                return Enumerable.Range(0, maximumNumberOfLocations).SelectCombination(maximumNumberOfLocations, Rng);
            }

            var locations = new List<int>();
            var i = 0;
            while (i < maximumNumberOfLocations)
            {
                // See if we will make a selection
                var mutate = Rng.NextDouble() < lambda;

                if (mutate)
                {
                    // See if we need to reduce the randomisation space
                    var offset = selectionWithReplacement
                        ? 0
                        : locations.Count;

                    // Generate a value 
                    var location = Rng.Next(0, numberOfLocationsToChooseFrom - offset);

                    if (selectionWithReplacement)
                    {
                        // We are generating with replacement - just add to list.
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
