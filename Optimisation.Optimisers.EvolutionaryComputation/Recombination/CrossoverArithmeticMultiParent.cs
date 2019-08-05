using System;
using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Performs an arithmetic (or 'flat') crossover: takes two or more parent <see cref="DecisionVector"/>s
    /// and creates a new one, by averaging each pair of elements from the two decision vectors together.
    /// See Jorge Magalhães-Mendes (2013) and Sivanandam and Deepa (2007)
    /// </summary>
    public class CrossoverArithmeticMultiParent : Operator, IMultiParentRecombinationOperator
    {
        /// <summary>
        /// Constructs a crossover operator to perform arithmetic (flat) two-parent crossover of two or more parents.
        /// </summary>
        public CrossoverArithmeticMultiParent() 
            : base("Arithmetic mean")
        {
        }

        /// <summary>
        /// Gets a new Decision Vector, based on an average of the matching elements from each parent.
        /// </summary>
        /// <param name="parents">A list of parent <see cref="DecisionVector"/>s.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if:
        /// - there are less than two parents; or
        /// - the parents have different length or zero length decision vectors; or
        /// - any of the parents have non-continuous Decision Vector elements.
        /// </exception>
        public DecisionVector Operate(IEnumerable<DecisionVector> parents)
        {
            var parentArray = parents as DecisionVector[] ?? parents.ToArray();
            
            if (parentArray.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(parents), 
                    "There must be at least two parents.");
            
            if (parentArray.Any(p => p.GetContinuousElements().Vector.Count == 0))
                throw new ArgumentOutOfRangeException(nameof(parents), 
                    "Parents must have non-zero length decision vectors.");

            if (parentArray.Any(p => p.GetContinuousElements().Vector.Count != parentArray.First().Vector.Count))
                throw new ArgumentOutOfRangeException(nameof(parents), 
                    "Parents must have the same length and fully continuous decision vectors.");

            return DecisionVector.CreateFromArray(
                parentArray.First().GetDecisionSpace(),
                parentArray
                    .Select(p => p.Vector.Select(v => (double) v))
                    .Aggregate((p1, p2) => p1.Select((v, i) => v + p2.ElementAt(i)))
                    .Select(v => v / parentArray.Length));
        }
    }
}