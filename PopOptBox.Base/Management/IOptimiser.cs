using System;
using System.Collections.Generic;

namespace PopOptBox.Base.Management
{
    /// <summary>
    /// The Optimiser handles the logic for deciding which individuals to try.
    /// As a metaheuristic, it only knows about the Decision Vector and the Fitness of each Individual
    /// </summary>
    public interface IOptimiser
    {
        /// <summary>
        /// Gets (up to) a certain number of individuals for evaluation
        /// </summary>
        /// <param name="numDesired">number of individuals wished for</param>
        /// <returns>List of Individual objects</returns>
        IReadOnlyList<Individual> GetNextToEvaluate(int numDesired);

        /// <summary>
        /// Handles reinsertion of evaluated individuals into the population
        /// </summary>
        /// <param name="individuals">List of Individuals</param>
        /// <returns>The number successfully reinserted.</returns>
        /// <exception cref="ArgumentException">One of the individuals is not yet evaluated.</exception>
        int ReInsert(IEnumerable<Individual> individuals);
    }
}