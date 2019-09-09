using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Base.PopulationCalculation
{
    /// <summary>
    /// Helper functions for creating initial <see cref="Management.Population"/>s.
    /// </summary>
    public static class PopulationCreators
    {
        /// <summary>
        /// Creates a set of Decision Vectors, based on a starting location, incrementing each dimension by a fixed amount.
        /// </summary>
        /// <param name="initialLocation">The <see cref="DecisionVector"/> representing the starting location.</param>
        /// <param name="stepSize">
        /// The distance from the starting location at which each further location will be created 
        /// (same in all dimensions).
        /// </param>
        /// <returns>List of <see cref="DecisionVector"/>s, the same length as the number of dimensions.</returns>
        public static List<DecisionVector> CreateNearLocation(DecisionVector initialLocation, double stepSize)
        {
            if (stepSize == 0)
                throw new System.ArgumentOutOfRangeException(nameof(stepSize), "Step size cannot be zero.");

            var newDVs = new List<DecisionVector>();

            var startDv = initialLocation.Select(d => (double)d).ToArray();

            for (var i = 2; i <= startDv.Length + 1; i++)
            {
                // Create D+1 total vertices.
                var newDv = new double[startDv.Length];
                startDv.CopyTo(newDv, 0);

                // Each vertex has one of its dimensions offset by an amount equal to stepsize.
                newDv[i - 2] += stepSize;

                try
                {
                    newDVs.Add(DecisionVector.CreateFromArray(
                        initialLocation.GetDecisionSpace(),
                        newDv));
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Can't go this way, as we are not in the acceptable region.
                    // Try to go the other way...
                    newDv[i - 2] -= 2 * stepSize;
                    
                    newDVs.Add(DecisionVector.CreateFromArray(
                        initialLocation.GetDecisionSpace(),
                        newDv));
                }
            }
            return newDVs;
        }

        /// <summary>
        /// Creates a set of Decision Vectors, which are generated as random legal solutions.
        /// </summary>
        /// <param name="space">The <see cref="DecisionSpace"/> defining what is legal.</param>
        /// <param name="numberToCreate">The number of new Decision Vectors desired.</param>
        /// <returns>A list of randomly located <see cref="DecisionVector"/>s.</returns>
        public static List<DecisionVector> CreateRandom(DecisionSpace space, int numberToCreate)
        {
            var newDVs = new List<DecisionVector>();

            var rng = new MathNet.Numerics.Random.MersenneTwister();

            for (var i = 1; i <= numberToCreate; i++)
            {
                var vector = new List<object>();
                foreach(var dim in space)
                {
                    vector.Add(dim.GetNextRandom(rng));
                }
                newDVs.Add(DecisionVector.CreateFromArray(space, vector));
            }

            return newDVs;
        }

        /// <summary>
        /// Creates a set of Decision Vectors, which are generated evenly across the decision space.
        /// </summary>
        /// <param name="space">The <see cref="DecisionSpace"/> defining what is legal.</param>
        /// <param name="numberToCreate">The number of new Decision Vectors desired.</param>
        /// <returns>A list of evenly-spaced <see cref="DecisionVector"/>s.</returns>
        public static List<DecisionVector> CreateLatinHypercube(DecisionSpace space, int numberToCreate)
        {
            var orderedLocations = space.Select(d => d.GetSpacedArray(numberToCreate));

            // TODO!
            throw new NotImplementedException();
        }
    }
}