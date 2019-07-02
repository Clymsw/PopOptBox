﻿using System;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Optimisers.NelderMead.Simplices
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
        /// <exception cref="ArgumentException">Thrown when the decision vector is not all continuous, or not the same number of dimensions</exception>
        public Simplex(IEnumerable<Individual> initialSimplex) :
            base(initialSimplex.First().DecisionVector.Vector.Count + 1, initialSimplex, constantLengthDv: true)
        {
            if (initialSimplex.Any(vx => 
                vx.DecisionVector.GetContinuousElements().Vector.Count < vx.DecisionVector.Vector.Count))
                throw new ArgumentException("All elements of the decision vector must be continuous for the Nelder-Mead optimiser", nameof(initialSimplex)); 
            
            // All vertices in the simplex must have the same number of dimensions
            // Checked by Population's check on DV length. 
            
            if (initialSimplex.Count() != initialSimplex.First().DecisionVector.Vector.Count + 1)
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

            var startDv = initialVertex.Vector.Select(d => (double)d).ToArray();

            for (var i = 2; i <= startDv.Length + 1; i++)
            {
                // Create D+1 total vertices.
                var newDv = new double[startDv.Length];
                startDv.CopyTo(newDv, 0);

                // Each vertex has one of its dimensions offset by an amount equal to stepsize.
                newDv[i - 2] += stepSize;

                simplex.Add(DecisionVector.CreateFromArray(
                    initialVertex.GetDecisionSpace(),
                    newDv));
            }
            return simplex;
        }
    }
}