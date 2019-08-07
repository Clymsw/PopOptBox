using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Calculation;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.NelderMead.Simplices
{
    /// <summary>
    /// The Simplex is a collection of Vertices (points in space: Individuals)
    /// which always has the length of D+1, where D is the number of dimensions.
    /// </summary>
    public class Simplex : Population
    {
        #region Constructors

        /// <summary>
        /// Creates a simplex with an initial set of locations
        /// </summary>
        /// <param name="initialSimplex">Array of <see cref="DecisionVector"/>s representing the simplex vertices.</param>
        /// <exception cref="ArgumentException">Thrown when the Decision Vector is not all continuous, or not the same number of dimensions</exception>
        public Simplex(IEnumerable<Individual> initialSimplex) :
            base(initialSimplex.First().DecisionVector.Count + 1, initialSimplex, constantLengthDv: true)
        {
            if (initialSimplex.Any(vx => 
                vx.DecisionVector.GetContinuousElements().Count < vx.DecisionVector.Count))
                throw new ArgumentException("All elements of the Decision Vector must be continuous for the Nelder-Mead optimiser", nameof(initialSimplex)); 
            
            // All vertices in the simplex must have the same number of dimensions
            // Checked by Population's check on DV length. 
            
            if (initialSimplex.Count() != initialSimplex.First().DecisionVector.Count + 1)
                throw new ArgumentException("The simplex must have D+1 elements",
                    nameof(initialSimplex));
        }

        /// <summary>
        /// Creates an empty Simplex.
        /// </summary>
        /// <param name="numberOfDimensions">Number of dimensions.</param>
        public Simplex(int numberOfDimensions) : base(numberOfDimensions + 1)
        {
        }

        #endregion

        /// <summary>
        /// Helper function to create an initial simplex for a given dimensionality around a starting location.
        /// </summary>
        /// <param name="initialVertex">The <see cref="DecisionVector"/> representing the starting location.</param>
        /// <param name="stepSize">The distance from the starting location at which each further vertex will be created (same in all dimensions).</param>
        /// <returns>List of vertices representing a valid Simplex.</returns>
        public static List<DecisionVector> CreateInitialVertices(
            DecisionVector initialVertex, double stepSize)
        {
            var simplex = new List<DecisionVector>
            {
                initialVertex
            };

            simplex.AddRange(PopulationCreators.CreateNearLocation(initialVertex, stepSize));

            return simplex;
        }
    }
}
