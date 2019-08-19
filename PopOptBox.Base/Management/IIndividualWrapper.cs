using System;
using System.Collections.Generic;

namespace PopOptBox.Base.Management
{
    /// <summary>
    /// Wraps an Individual inside a <see cref="Population"/>
    /// Provides additional information about its relative performance.
    /// <seealso cref="IIndividualPerformance"/>.
    /// </summary>
    public interface IIndividualWrapper : IComparable
    {
        /// <summary>
        /// Provides the <see cref="Individual"/> contained in the wrapper.
        /// </summary>
        /// <returns>The Individual.</returns>
        Individual GetIndividual();
        
        /// <summary>
        /// Gets the fitness (if set).
        /// </summary>
        /// <returns>The Fitness.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the fitness hasn't yet been set.</exception>
        double GetFitness();
        
        /// <summary>
        /// Sets the fitness, where applicable relative to the other <see cref="Individual"/>s passed in,
        /// intended to be (a subset of) the <see cref="Population"/>..
        /// </summary>
        /// <param name="performanceCalculator">The calculator to assess fitness.</param>
        /// <param name="others">The other individuals to be compared against.</param>
        void SetFitness(IIndividualPerformance performanceCalculator, IEnumerable<Individual> others = null);
    }
}