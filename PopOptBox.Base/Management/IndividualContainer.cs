using System;
using System.Collections.Generic;
using System.Linq;

namespace PopOptBox.Base.Management
{
    /// <summary>
    /// Container for <see cref="Management.Individual"/>s inside a <see cref="Population"/>.
    /// Extra information, e.g. relative strength compared with other individuals in the population. 
    /// </summary>
    internal class IndividualContainer
    {
        public readonly Individual TheIndividual;
        
        public IndividualContainer(Individual theIndividual)
        {
            TheIndividual = theIndividual;
            Dominating = new List<Individual>();
            DominatedBy = new List<Individual>();
        }

        #region Properties

        /// <summary>
        /// The <see cref="Management.Individual"/>s which this individual currently dominates.
        /// Managed by <see cref="Optimiser"/>.
        /// <seealso cref="IsDominating(Management.Individual)"/>
        /// </summary>
        public readonly List<Individual> Dominating;

        /// <summary>
        /// The <see cref="Management.Individual"/>s which are currently dominating this individual.
        /// Managed by <see cref="Optimiser"/>.
        /// <seealso cref="IsDominatedBy(Management.Individual)"/>
        /// </summary>
        public readonly List<Individual> DominatedBy;

        /// <summary>
        /// The Pareto Front rank of this <see cref="Management.Individual"/> in the current <see cref="Population"/>.
        /// </summary>
        public int Rank => DominatedBy.Count;
        
        #endregion

        #region Pareto
        
        /// <summary>
        /// Gets whether another Individual strictly dominates this one.
        /// </summary>
        /// <param name="other">The other Individual to compare.</param>
        /// <returns><see langword="true"/> if this individual is dominated.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the two Solution Vectors have different lengths.</exception>
        public bool IsDominatedBy(Individual other)
        {
            if (other.SolutionVector.Length != TheIndividual.SolutionVector.Length)
                throw new ArgumentOutOfRangeException(nameof(other), 
                    "Other individual must have the same number of objectives in its Solution Vector.");
            
            return other.SolutionVector.Select((v, i) => 
                v < TheIndividual.SolutionVector.ElementAt(i)).All(b => b);
        }

        /// <summary>
        /// Gets whether this Individual strictly dominates another one.
        /// </summary>
        /// <param name="other">The other Individual to compare.</param>
        /// <returns><see langword="true"/> if the other individual is dominated.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the two Solution Vectors have different lengths.</exception>
        public bool IsDominating(Individual other)
        {
            if (other.SolutionVector.Length != TheIndividual.SolutionVector.Length)
                throw new ArgumentOutOfRangeException(nameof(other), 
                    "Other individual must have the same number of objectives in its Solution Vector.");
            
            return TheIndividual.SolutionVector.Select((v, i) => 
                v < other.SolutionVector.ElementAt(i)).All(b => b);
        }
        
        #endregion

        #region ToString

        public override string ToString()
        {
            return $"R={Rank} F=" + TheIndividual.ToString();
        }

        #endregion

        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (!(obj is IndividualContainer other))
                return false;
            
            return TheIndividual.Equals(other.TheIndividual);
        }

        public override int GetHashCode()
        {
            return TheIndividual.GetHashCode();
        }
        
        #endregion
    }
}