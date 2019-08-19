using System;
using System.Collections.Generic;
using PopOptBox.Base.Calculation;

namespace PopOptBox.Base.Management
{
    /// <summary>
    /// Implementation of an individual wrapper which handles absolute Fitnesses used for single objective optimisation.
    /// </summary>
    public class IndividualWrapperSingleObjective : IIndividualWrapper
    {
        private readonly Individual individual;
        
        /// <summary>
        /// The Fitness, used to rank individuals and ultimately determine optimality.
        /// Lower is better.
        /// </summary>
        private double? fitness;
        
        /// <summary>
        /// Constructs the individual wrapper.
        /// </summary>
        /// <param name="individual">The individual to wrap.</param>
        public IndividualWrapperSingleObjective(Individual individual)
        {
            this.individual = individual;
        }

        /// <summary>
        /// Gets the contained <see cref="Individual"/>.
        /// </summary>
        /// <returns>The individual.</returns>
        public Individual GetIndividual()
        {
            return individual;
        }
        
        /// <summary>
        /// Gets the Fitness set by <see cref="SetFitness"/>.
        /// </summary>
        /// <returns>The Fitness.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the Fitness is not yet set.</exception>
        public double GetFitness()
        {
            if (fitness == null) 
                throw new InvalidOperationException("Fitness not yet set. Use SetFitness().");
            
            return fitness.Value;
        }

        /// <summary>
        /// Assigns Fitness based on the calculator provided.
        /// </summary>
        /// <remarks>Can be performed more than once.</remarks>
        /// <param name="performanceCalculator">A calculator such as <see cref="PopOptBox.Base.Calculation.FitnessSingleObjective"/>.</param>
        /// <param name="others">Other individuals. Not relevant, not used.</param>
        /// <exception cref="InvalidOperationException">Thrown when contained <see cref="Individual"/> is not evaluated.</exception>
        public void SetFitness(
            IIndividualPerformanceCalculator performanceCalculator, 
            IEnumerable<Individual> others = null)
        {
            if (individual.State != IndividualState.Evaluated)
                throw new InvalidOperationException("Individual is not evaluated!");
            
            fitness = performanceCalculator.Calculate(individual, others);
        }
        
        #region IComparable

        public int CompareTo(object obj)
        {
            if (!(obj is IIndividualWrapper other))
                return -1; // This is lower (better)
                    
            return other.GetFitness() > GetFitness()
                ? -1 // This is lower (better) 
                : Math.Abs(other.GetFitness() - GetFitness()) < 1e-9
                    ? 0// This is equal
                    : -1; // This is higher (worse)
        }

        #endregion

        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (!(obj is IIndividualWrapper other))
                return false;
            
            return individual.Equals(other.GetIndividual());
        }

        public override int GetHashCode()
        {
            return new
            {
                individual
            }.GetHashCode();
        }

        #endregion
    }
}