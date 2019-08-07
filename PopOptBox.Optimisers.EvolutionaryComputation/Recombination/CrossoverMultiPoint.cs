using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Performs the multi-point crossover: takes two parent <see cref="DecisionVector"/>s and creates a new one,
    /// by selecting alternating sections from each parent.
    /// See Jorge Magalhães-Mendes (2013), Goldberg (1989) and de Jong (1992)
    /// </summary>
    public class CrossoverMultiPoint : Operator, ITwoParentCrossoverOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly int numberOfCrossoverLocations; 

        /// <summary>
        /// Constructs a crossover operator to perform multi-point two-parent crossover.
        /// </summary>
        public CrossoverMultiPoint(int numberOfCrossoverLocations = 1) 
            : base($"{numberOfCrossoverLocations}-point (not permutation-safe)")
        {
            rngManager = new RandomNumberManager();
            this.numberOfCrossoverLocations = numberOfCrossoverLocations;
        }

        /// <summary>
        /// Gets a new Decision Vector, based on selecting a point at random 
        /// and copying values from one parent's Decision Vector before that point,
        /// then the values from the other parent's Decision Vector after that point.
        /// The order the parents are provided does not matter, they are selected at random to ensure uniform probability distribution over outcomes.
        /// </summary>
        /// <remarks>
        /// All outcomes are equally likely, including that one or the other parent is returned complete.
        /// The parents can have different length decision vectors; the parent with the correct length <see cref="DecisionSpace"/> is used.
        /// </remarks>
        /// <param name="firstParent">One <see cref="DecisionVector"/> to use as a parent.</param>
        /// <param name="secondParent">Another <see cref="DecisionVector"/> to use as a parent.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the crossed-over values are not accepted by the decision space.</exception>
        public DecisionVector Operate(DecisionVector firstParent, DecisionVector secondParent)
        {
            // Choose one or other parent as first, at random.
            var parents = rngManager.Rng.NextBoolean() 
                ? new[] {secondParent, firstParent} 
                : new[] {firstParent, secondParent};

            // Select a crossover location
            // This lies in between vector elements, hence Count + 1
            // The vectors might be different lengths, so select the shortest one.
            var crossoverPoints = rngManager.GetLocations(
                firstParent.Count > secondParent.Count
                    ? secondParent.Count + 1
                    : firstParent.Count + 1,
                numberOfCrossoverLocations, 
                false, 
                1).ToList();
            crossoverPoints.Add(0);
            crossoverPoints.Sort();

            var newVector = new List<object>();
            var parentIdx = 0; 
            for (var i = 1; i < crossoverPoints.Count; i++)
            {
                var numPointsToTake = crossoverPoints.ElementAt(i) - crossoverPoints.ElementAt(i - 1);
                // Add elements to the new Decision Vector
                newVector.AddRange(parents.ElementAt(parentIdx)
                    .Skip(crossoverPoints.ElementAt(i - 1))
                    .Take(numPointsToTake));
                
                // Flip parent
                parentIdx++;
                if (parentIdx > 1)
                    parentIdx = 0;
            }

            newVector.AddRange(parents.ElementAt(parentIdx)
                .Skip(crossoverPoints.Last()));

            return DecisionVector.CreateFromArray(
                firstParent.Count == newVector.Count 
                    ? firstParent.GetDecisionSpace()
                    : secondParent.GetDecisionSpace(), 
                newVector);
        }
    }
}
