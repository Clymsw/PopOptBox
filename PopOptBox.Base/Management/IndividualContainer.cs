using System;
using System.Collections.Generic;
using System.Linq;

namespace PopOptBox.Base.Management
{
    internal class IndividualContainer
    {
        private readonly Individual individual;
        
        public IndividualContainer(Individual individual)
        {
            this.individual = individual;
            Dominating = new List<Individual>();
            DominatedBy = new List<Individual>();
        }

        #region Properties

        /// <summary>
        /// The <see cref="Individual"/>s which this individual currently dominates.
        /// Managed by <see cref="Optimiser"/>.
        /// <seealso cref="IsDominating(Individual)"/>
        /// </summary>
        public readonly List<Individual> Dominating;

        /// <summary>
        /// The <see cref="Individual"/>s which are currently dominating this individual.
        /// Managed by <see cref="Optimiser"/>.
        /// <seealso cref="IsDominatedBy(Individual)"/>
        /// </summary>
        public readonly List<Individual> DominatedBy;

        /// <summary>
        /// The Pareto Front rank of this <see cref="Individual"/> in the current <see cref="Population"/>.
        /// </summary>
        public int Rank => DominatedBy.Count;
        
        #endregion
        
        /// <summary>
        /// Gets whether another Individual strictly dominates this one.
        /// </summary>
        /// <param name="other">The other Individual to compare.</param>
        /// <returns><see langword="true"/> if this individual is dominated.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the two Solution Vectors have different lengths.</exception>
        public bool IsDominatedBy(Individual other)
        {
            if (other.SolutionVector.Length != individual.SolutionVector.Length)
                throw new ArgumentOutOfRangeException(nameof(other), 
                    "Other individual must have the same number of objectives in its Solution Vector.");
            
            return other.SolutionVector.Select((v, i) => 
                v < individual.SolutionVector.ElementAt(i)).All(b => b);
        }

        /// <summary>
        /// Gets whether this Individual strictly dominates another one.
        /// </summary>
        /// <param name="other">The other Individual to compare.</param>
        /// <returns><see langword="true"/> if the other individual is dominated.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the two Solution Vectors have different lengths.</exception>
        public bool IsDominating(Individual other)
        {
            if (other.SolutionVector.Length != individual.SolutionVector.Length)
                throw new ArgumentOutOfRangeException(nameof(other), 
                    "Other individual must have the same number of objectives in its Solution Vector.");
            
            return individual.SolutionVector.Select((v, i) => 
                v < other.SolutionVector.ElementAt(i)).All(b => b);
        }
    }
}