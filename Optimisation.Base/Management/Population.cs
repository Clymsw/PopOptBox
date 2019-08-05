using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Base.Management
{
    /// <summary>
    /// The Population contains and manages a number of solutions for an optimisation problem.
    /// <seealso cref="Individual"/>.
    /// </summary>
    public class Population : IReadOnlyCollection<Individual>
    {
        #region Fields

        private readonly List<Individual> members;

        /// <summary>
        /// Whether or not one should expect every individual to have the same length of <see cref="Variables.DecisionSpace"/>. 
        /// </summary>
        public readonly bool ConstantLengthDecisionVector;
        
        /// <summary>
        /// The targeted maximum number of <see cref="Individual"/>s in the population.
        /// </summary>
        public readonly int TargetSize;

        /// <summary>
        /// Whether we have met or exceeded our desired size of population.
        /// </summary>
        public bool IsTargetSizeReached => members.Count >= TargetSize;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialise population with a target size and an initial array of individuals
        /// </summary>
        /// <param name="initialPopulation">An array of individuals</param>
        /// <param name="initialSize">Expected max size of population</param>
        /// <param name="constantLengthDv">Whether the decision vector should be expected to be constant</param>
        public Population(
            int initialSize = 100,
            IEnumerable<Individual> initialPopulation = null,
            bool constantLengthDv = true)
        {
            if (initialSize > 0)
                TargetSize = initialSize;

            ConstantLengthDecisionVector = constantLengthDv;

            members = new List<Individual>(TargetSize);

            if (initialPopulation == null)
                return;

            foreach (var ind in initialPopulation) AddIndividual(ind);
        }

        /// <summary>
        /// Returns a shallow copy of the Population.
        /// </summary>
        public virtual Population Clone()
        {
            return new Population(
                TargetSize,
                members.Select(m => m.Clone()),
                ConstantLengthDecisionVector);
        }

        #endregion

        #region Information Extraction

        /// <summary>
        /// An indexer to allow direct indexation to the Population 
        /// class, which will return the individual of interest.
        /// </summary>
        /// <param name="index">Integer index into population.</param>
        /// <returns>Individual</returns>
        public Individual this[int index] => members[index];

        /// <summary>
        /// Allows read-only access to the individuals
        /// </summary>
        /// <returns>List of <see cref="Individual"/>s.</returns>
        public IReadOnlyList<Individual> GetMemberList()
        {
            return members;
        }
        
        /// <summary>
        /// Gets the individual with the best (lowest) fitness.
        /// </summary>
        /// <returns>An evaluated <see cref="Individual"/> with the best fitness.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when the population is empty.</exception>
        public Individual Best()
        {
            return members.Count > 0
                ? members[0]
                : throw new InvalidOperationException("Population has no members.");
        }

        /// <summary>
        /// Gets the individual with the worst (highest) fitness.
        /// </summary>
        /// <returns>An evaluated <see cref="Individual"/> with the worst fitness.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when the population is empty.</exception>
        public Individual Worst()
        {
            return members.Count > 0
                ? members[members.Count - 1]
                : throw new InvalidOperationException("Population has no members.");
        }

        /// <summary>
        /// Gets the individuals on the Pareto front.
        /// </summary>
        /// <returns>A (read-only) list of <see cref="Individual"/>s.</returns>
        public IReadOnlyList<Individual> ParetoFront()
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the Fitness values of all individuals
        /// </summary>
        /// <returns>List of doubles: Fitness</returns>
        public IEnumerable<double> GetMemberFitnesses()
        {
            return members.Select(i => i.Fitness);
        }

        /// <summary>
        /// Get the Solution Vectors of all individuals
        /// </summary>
        /// <returns>List of double arrays: Solution Vector</returns>
        public IEnumerable<double[]> GetMemberSolutionVectors()
        {
            return members.Select(i => i.SolutionVector);
        }

        /// <summary>
        /// Get the DVs of all individuals
        /// </summary>
        /// <returns>List of object arrays: Decision Vector</returns>
        public IEnumerable<object[]> GetMemberDecisionVectors()
        {
            return members.Select(i => i.DecisionVector.Vector.ToArray());
        }
        
        /// <summary>
        /// Calculates the difference in fitness between the best and worst individuals in the population.
        /// </summary>
        /// <returns>The fitness range for the population</returns>
        public double FitnessRange() => Worst().Fitness - Best().Fitness;
        
        /// <summary>
        /// Calculates the difference in fitness between the best and worst individuals in the population.
        /// </summary>
        /// <returns>The fitness range for the population</returns>
        public double FitnessAverage() => GetMemberFitnesses().Average();

        /// <summary>
        /// Calculates the absolute differences in decision vector values between the best and worst individuals in the population.
        /// </summary>
        /// <returns>The range of each variable in the decision space</returns>
        /// <exception cref="ArgumentException">If the individuals do not have the same decision space.</exception>
        public IEnumerable<double> DecisionVectorRangeByFitness() => 
            (Worst().DecisionVector - Best().DecisionVector).Select(Math.Abs); 
        
        #endregion

        #region Management

        /// <summary>
        /// Adds an <see cref="Individual"/> to the population.
        /// </summary>
        /// <param name="ind">The individual to add.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if: 
        /// 1) we are expecting individuals to have the same length Decision Vector and it is not true; 
        /// 2) The Individual is not yet evaluated.
        /// </exception>
        public void AddIndividual(Individual ind)
        {
            if (ConstantLengthDecisionVector && members.Count > 0)
                if (ind.DecisionVector.Vector.Count != members[0].DecisionVector.Vector.Count)
                    throw new ArgumentException(
                        "Decision Vector is not the right length!");
            
            if (ind.State != IndividualState.Evaluated)
                throw new ArgumentException("Individual is not yet evaluated.");
            
            // Add to population
            members.Add(ind);

            Sort();
        }

        /// <summary>
        /// Sorts array by fitness, best (lowest!) at top.
        /// </summary>
        private void Sort()
        {
            members.Sort((p, q) => p.Fitness.CompareTo(q.Fitness));
        }
        
        /// <summary>
        /// Replaces worst individual with a new one.
        /// </summary>
        /// <param name="individualToInsert">New individual to insert.</param>
        /// <exception cref="System.InvalidOperationException">Thrown when the population is empty.</exception>
        public void ReplaceWorst(Individual individualToInsert)
        {
            if (members.Count == 0)
                throw new InvalidOperationException("Population has no members.");

            members.Remove(Worst());
            AddIndividual(individualToInsert);
        }

        /// <summary>
        /// Replaces individual with a particular index with a new one.
        /// </summary>
        /// <param name="index">The index of the individual to remove.</param>
        /// <param name="individualToInsert">New individual to insert.</param>
        /// <exception cref="InvalidOperationException">Thrown when the population is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is not valid.</exception>
        public void ReplaceIndividualAt(int index, Individual individualToInsert)
        {
            if (members.Count == 0)
                throw new InvalidOperationException("Population has no members.");
            
            if (index < 0 || index >= members.Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"{index} is not a valid index into the population.");
            
            members.RemoveAt(index);
            
            AddIndividual(individualToInsert);
        }

        /// <summary>
        /// Replaces a particular individual with a new one.
        /// </summary>
        /// <param name="individualToRemove">Old individual to remove.</param>
        /// <param name="individualToInsert">New individual to insert.</param>
        /// <exception cref="InvalidOperationException">Thrown when the population is empty, or old individual is not present.</exception>
        public void ReplaceIndividual(Individual individualToRemove, Individual individualToInsert)
        {
            if (members.Count == 0)
                throw new InvalidOperationException("Population has no members.");

            var removedOk = members.Remove(individualToRemove);
            if (!removedOk)
                throw new InvalidOperationException("Old individual was not found in population.");

            AddIndividual(individualToInsert);
        }

        #endregion

        #region Implementation of IEnumerable

        public IEnumerator<Individual> GetEnumerator()
        {
            return members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) members).GetEnumerator();
        }

        #endregion

        #region Implementation of IReadOnlyCollection<out Individual<TDecVec>>

        public int Count => members.Count;

        #endregion
    }
}