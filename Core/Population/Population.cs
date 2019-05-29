using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Core.Population
{
    /// <summary>
    /// The Population contains a number of possible solutions ("Individuals")
    /// for an optimisation problem.
    /// </summary>
    /// <typeparam name="TDecVec">The type of the Decision Vector, 
    /// e.g. int for a Genetic Algorithm</typeparam>
    public class Population<TDecVec> : IReadOnlyCollection<Individual<TDecVec>>
    {
        #region Fields

        // A list of all members of the population.
        private readonly List<Individual<TDecVec>> members;

        private readonly int initialSize;

        public readonly bool ConstantLengthDecisionVector;

        public bool IsTargetSizeReached => members.Count >= initialSize;

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
            IEnumerable<Individual<TDecVec>> initialPopulation = null,
            bool constantLengthDv = true)
        {
            if (initialSize > 0)
                this.initialSize = initialSize;

            ConstantLengthDecisionVector = constantLengthDv;

            members = new List<Individual<TDecVec>>(this.initialSize);

            if (initialPopulation == null)
                return;

            foreach (var ind in initialPopulation) AddIndividual(ind);
        }

        /// <summary>
        /// Returns a shallow copy of the Population
        /// (member list is copied)
        /// </summary>
        public virtual Population<TDecVec> Clone()
        {
            return new Population<TDecVec>(
                initialSize,
                members.ToArray(),
                ConstantLengthDecisionVector);
        }

        #endregion

        #region FakeProperties

        /// <summary>
        /// An indexer to allow direct indexation to the Population 
        /// class, which will return the individual of interest 
        /// </summary>
        /// <param name="index">Integer index into population</param>
        /// <returns>Individual</returns>
        public Individual<TDecVec> this[int index] => members[index];

        public IReadOnlyList<Individual<TDecVec>> GetMemberList()
        {
            return members;
        }

        public Individual<TDecVec> Best()
        {
            return members[0];
        }

        public Individual<TDecVec> Worst()
        {
            return members[members.Count - 1];
        }

        /// <summary>
        /// Get the Fitnesses of all individuals
        /// </summary>
        /// <returns>List of doubles: Fitness</returns>
        public List<double> ListFitness()
        {
            return members.Select(i => i.Fitness).ToList();
        }

        /// <summary>
        /// Get the Scores of all individuals
        /// </summary>
        /// <returns>List of double arrays: Score</returns>
        public List<double[]> ListScore()
        {
            return members.Select(i => i.Score).ToList();
        }

        /// <summary>
        /// Get the Solution Vectors of all individuals
        /// </summary>
        /// <returns>List of double arrays: Solution Vector</returns>
        public List<double[]> ListSolution()
        {
            return members.Select(i => i.SolutionVector).ToList();
        }

        /// <summary>
        /// Get the DVs of all individuals
        /// </summary>
        /// <returns>List of TDecVec arrays: DV</returns>
        public List<TDecVec[]> ListDecision()
        {
            return members.Select(i => i.DecisionVector.ToArray()).ToList();
        }

        #endregion

        #region Management

        /// <summary>
        /// Adds an individual to the population
        /// </summary>
        /// <param name="ind">The individual to add</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddIndividual(Individual<TDecVec> ind)
        {
            if (ConstantLengthDecisionVector && members.Count > 0)
                if (ind.DecisionVector.Count != members[0].DecisionVector.Count)
                    throw new ArgumentException(
                        "Decision Vectors not the same length!");

            if (IsTargetSizeReached)

                // Add to population
                members.Add(ind);

            Sort();
        }

        /// <summary>
        /// Replaces worst individual with a new one
        /// </summary>
        /// <param name="ind">New individual to insert</param>
        public void ReplaceWorst(Individual<TDecVec> ind)
        {
            members[members.Count - 1] = ind;
            Sort();
        }

        /// <summary>
        /// Sorts array by fitness, best (lowest!) at top.
        /// </summary>
        private void Sort()
        {
            members.Sort((p, q) => p.Fitness.CompareTo(q.Fitness));
        }

        #endregion

        #region Implementation of IEnumerable

        /// <inheritdoc />
        public IEnumerator<Individual<TDecVec>> GetEnumerator()
        {
            return members.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) members).GetEnumerator();
        }

        #endregion

        #region Implementation of IReadOnlyCollection<out Individual<TDecVec>>

        /// <inheritdoc />
        public int Count => members.Count;

        #endregion
    }
}