using System;
using System.Globalization;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Performs an arithmetic (or 'flat') crossover: takes two parent <see cref="DecisionVector"/>s
    /// and creates a new one, by performing a weighted average of each pair of elements from the two decision vectors.
    /// See Jorge Magalhães-Mendes (2013) and Sivanandam and Deepa (2007)
    /// </summary>
    public class CrossoverArithmeticWeighted : Operator, IRecombinationOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly bool allRandomWeights;
        private readonly double fixedWeight;
        
        /// <summary>
        /// Constructs a crossover operator to perform weighted arithmetic (flat) two-parent crossover.
        /// </summary>
        /// <param name="allRandomWeights"><see langword="false"/> if weights are fixed to one value, specified by <see cref="fixedWeight"/>.</param>
        /// <param name="fixedWeight">The value to weight the first parent's Decision Vector elements by when summing them.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the fixed weight is not between 0 and 1.</exception>
        public CrossoverArithmeticWeighted(bool allRandomWeights = false, double fixedWeight = 0.5) 
            : base(allRandomWeights 
                ? $"Arithmetic (random weights)" 
                : $"Arithmetic (weight = {fixedWeight.ToString("F2", CultureInfo.InvariantCulture)})")
        {
            if (!allRandomWeights && (fixedWeight < 0 || fixedWeight > 1))
                throw new ArgumentOutOfRangeException(nameof(fixedWeight), "Weight must be between 0 and 1");
            
            rngManager = new RandomNumberManager();
            this.allRandomWeights = allRandomWeights;
            this.fixedWeight = fixedWeight;
        }

        /// <summary>
        /// Gets a new Decision Vector, based on a weighted average of the matching elements from each parent.
        /// This sum can be biased towards the first parent (if <see cref="fixedWeight"/> is greater than 0.5)
        /// or towards the second parent (if <see cref="fixedWeight"/> is less than 0.5). 
        /// </summary>
        /// <remarks>
        /// The vectors must be the same length.
        /// </remarks>
        /// <param name="parents">Two <see cref="DecisionVector"/>s to use as a parents.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the parents are not continuous decision vectors or not the same length.</exception>
        public DecisionVector Operate(params DecisionVector[] parents)
        {
            var firstParent = parents[0];
            var secondParent = parents[1];

            if (firstParent.GetContinuousElements().Count != firstParent.Count
                || firstParent.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(firstParent),
                    "This operator can only be used on continuous variable decision vectors of non-zero length.");
            
            if (firstParent.Count !=
                secondParent.GetContinuousElements().Count)
                throw new ArgumentOutOfRangeException(nameof(secondParent),
                    "Both parents must have the same length (continuous) decision vectors");
            
            if (!allRandomWeights)
            {
                return DecisionVector.CreateFromArray(
                    firstParent.GetDecisionSpace(),
                    firstParent.Select(
                        (v, i) => fixedWeight * (double) v
                                  + (1 - fixedWeight) * (double) secondParent.ElementAt(i)));
            }
            
            return DecisionVector.CreateFromArray(
                firstParent.GetDecisionSpace(),
                firstParent.Select(
                    (v, i) =>
                    {
                        var weight = rngManager.Rng.NextDouble(); 
                        return weight * (double) v
                               + (1 - weight) * (double) secondParent.ElementAt(i);
                    }));
        }
    }
}