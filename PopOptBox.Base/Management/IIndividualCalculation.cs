using System.Collections.Generic;

namespace PopOptBox.Base.Management
{
    /// <summary>
    /// Interface for optimisation-relevant calculations of an <see cref="Individual"/>'s performance,
    /// relative to (some sub-set of) the rest of the <see cref="Population"/>.
    /// </summary>
    public interface IIndividualCalculation
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