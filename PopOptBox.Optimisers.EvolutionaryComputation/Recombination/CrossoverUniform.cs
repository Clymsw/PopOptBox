using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Performs the uniform crossover: takes two parent <see cref="DecisionVector"/>s and creates a new one,
    /// by selecting each element at random from one of the parents.
    /// See Jorge Magalhães-Mendes (2013) and Gonçalves et al. (2005)
    /// </summary>
    public class CrossoverUniform : Operator, IRecombinationOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly double crossoverBias;

        /// <summary>
        /// Constructs a crossover operator to perform uniform two-parent crossover.
        /// </summary>
        /// <param name="crossoverBias">The bias towards the first parent, by default 0.5 (unbiased).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <see cref="crossoverBias"/> is not a legal probability.</exception>
        public CrossoverUniform(double crossoverBias = 0.5)
            : base($"Uniform (first parent chosen with probability {crossoverBias.ToString("F2", CultureInfo.InvariantCulture)})")
        {
            if (crossoverBias < 0.0 || crossoverBias > 1.0)
                throw new ArgumentOutOfRangeException(nameof(crossoverBias),
                    "Parent bias probability must be a value between 0 and 1");

            rngManager = new RandomNumberManager();
            this.crossoverBias = crossoverBias;
        }

        /// <summary>
        /// Gets a new Decision Vector, based on selecting each element from one parent or the other.
        /// This selection can be biased towards the first parent (if <see cref="crossoverBias"/> is greater than 0.5)
        /// or towards the second parent (if <see cref="crossoverBias"/> is less than 0.5). 
        /// </summary>
        /// <remarks>
        /// The two decision vectors can be different lengths.
        /// If so, for each element that only exists in the longer parent, if the shorter parent is selected,
        /// that element is removed. 
        /// </remarks>
        /// <param name="parents">Two <see cref="DecisionVector"/>s to use as a parents.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        public DecisionVector Operate(params DecisionVector[] parents)
        {
            var firstParent = parents[0];
            var secondParent = parents[1];
            
            var numDims = Math.Max(firstParent.Count, secondParent.Count);

            var dims = new List<IVariable>();
            var newVector = new List<object>();
            for (var d = 0; d < numDims; d++)
            {
                var parentToUse = rngManager.Rng.NextDouble() < crossoverBias
                    ? firstParent
                    : secondParent;

                if (parentToUse.Count < (d + 1)) 
                    continue;
                
                dims.Add(parentToUse.GetDecisionSpace().ElementAt(d));
                newVector.Add(parentToUse.ElementAt(d));
            }
            
            return DecisionVector.CreateFromArray(new DecisionSpace(dims), newVector);
        }
    }
}