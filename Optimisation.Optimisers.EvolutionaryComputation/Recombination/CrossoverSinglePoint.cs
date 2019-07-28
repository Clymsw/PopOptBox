using Optimisation.Base.Variables;
using System;
using System.Linq;

namespace Optimisation.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Performs the single-point crossover: takes two parent <see cref="DecisionVector"/>s and creates a new one,
    /// by selecting the beginning section from one and the end section from the other.
    /// See Jorge Magalhães-Mendes (2013) and Goldberg (1989)
    /// </summary>
    public class CrossoverSinglePoint : Operator, ITwoParentCrossoverOperator
    {
        private readonly RandomNumberManager rngManager;

        /// <summary>
        /// Constructs a crossover operator to perform single-point two-parent crossover.
        /// </summary>
        public CrossoverSinglePoint() : base("Swap vector elements around a randomly-chosen single point")
        {
            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a new decision vector, based on selected a point at random 
        /// and copying values from one parent's decision vector before that point,
        /// then the values from the other parent's decision vector after that point.
        /// The order the parents are provided does not matter, they are re-ordered at random to ensure uniform probability distribution over outcomes.
        /// </summary>
        /// <remarks>
        /// All outcomes are equally likely, including that one or the other parent is returned complete.
        /// </remarks>
        /// <param name="firstParent">One <see cref="DecisionVector"/> to use as a parent.</param>
        /// <param name="secondParent">Another <see cref="DecisionVector"/> to use as a parent.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the crossed-over values are not accepted by the decision space.</exception>
        public DecisionVector Operate(DecisionVector firstParent, DecisionVector secondParent)
        {
            // Choose one or other parent as first, at random.
            DecisionVector item1, item2;
            if (rngManager.Rng.NextDouble() < 0.5)
            {
                item1 = secondParent;
                item2 = firstParent;
            }
            else
            {
                item2 = secondParent;
                item1 = firstParent;
            }

            // Select a crossover location
            var crossoverPoint = rngManager.Rng.Next(0, firstParent.Vector.Count);

            // Create the new Decision Vector
            var newVector = item1.Vector.Take(crossoverPoint).ToList();
            newVector.AddRange(item2.Vector.Skip(crossoverPoint));

            return DecisionVector.CreateFromArray(
                firstParent.GetDecisionSpace(),
                newVector);
        }
    }
}
