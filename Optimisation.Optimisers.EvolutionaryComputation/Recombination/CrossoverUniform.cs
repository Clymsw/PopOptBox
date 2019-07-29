using System;
using System.Linq;
using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Performs the uniform crossover: takes two parent <see cref="DecisionVector"/>s and creates a new one,
    /// by selecting each element at random from one of the parents.
    /// See Jorge Magalhães-Mendes (2013) and Gonçalves et al. (2005)
    /// </summary>
    public class CrossoverUniform : Operator, ITwoParentCrossoverOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly double crossoverBias;
        
        /// <summary>
        /// Constructs a crossover operator to perform uniform two-parent crossover.
        /// </summary>
        /// <param name="crossoverBias">The bias towards the first parent, by default 0.5 (unbiased).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <see cref="crossoverBias"/> is not a legal probability.</exception>
        public CrossoverUniform(double crossoverBias = 0.5) 
            : base($"Choose value from first parent with probability {crossoverBias}")
        {
            if (crossoverBias < 0.0 || crossoverBias > 1.0)
                throw new ArgumentOutOfRangeException(nameof(crossoverBias),
                    "Parent bias probability must be a value between 0 and 1");

            rngManager = new RandomNumberManager();
            this.crossoverBias = crossoverBias;
        }

        /// <summary>
        /// Gets a new decision vector, based on selecting each element from one parent or the other.
        /// This selection can be biased towards the first parent (if <see cref="crossoverBias"/> is greater than 0.5)
        /// or towards the second parent (if <see cref="crossoverBias"/> is less than 0.5). 
        /// </summary>
        /// <param name="firstParent">One <see cref="DecisionVector"/> to use as a parent.</param>
        /// <param name="secondParent">Another <see cref="DecisionVector"/> to use as a parent.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        public DecisionVector Operate(DecisionVector firstParent, DecisionVector secondParent)
        {
            var newVector = firstParent.Vector.Select(
                (v,i) => rngManager.Rng.NextDouble() < crossoverBias
                         ? v
                         : secondParent.Vector.ElementAt(i)); 
            
            return DecisionVector.CreateFromArray(firstParent.GetDecisionSpace(),
                newVector);
        }
    }
}