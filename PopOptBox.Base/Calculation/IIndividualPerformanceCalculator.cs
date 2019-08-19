using System.Collections.Generic;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Calculation
{
    /// <summary>
    /// Interface for optimisation-relevant calculations of an <see cref="Individual"/>'s performance,
    /// relative to (some sub-set of) the rest of the <see cref="Population"/>.
    /// </summary>
    public interface IIndividualPerformanceCalculator
    {
        /// <summary>
        /// Perform the calculation of performance
        /// </summary>
        /// <param name="individualOfInterest"></param>
        /// <param name="restOfPopulation"></param>
        /// <returns></returns>
        double Calculate(Individual individualOfInterest, IEnumerable<Individual> restOfPopulation);
    }
}