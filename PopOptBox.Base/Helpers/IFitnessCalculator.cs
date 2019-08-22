using System.Collections.Generic;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Helpers
{
    public interface IFitnessCalculator
    {
        /// <summary>
        /// Performs the fitness calculation using the contract specified by
        /// the constructor of an <see cref="Optimiser"/>.
        /// </summary>
        /// <param name="individualOfInterest">The individual which needs its fitness assigned.</param>
        /// <param name="restOfPopulation">The other individuals which should be taken into consideration when assigning fitness.</param>
        void CalculateAndAssignFitness(
            Individual individualOfInterest,
            IEnumerable<Individual> restOfPopulation);
    }
}