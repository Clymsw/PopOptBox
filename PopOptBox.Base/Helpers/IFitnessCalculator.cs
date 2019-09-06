using System.Collections.Generic;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Helpers
{
    public interface IFitnessCalculator
    {
        /// <summary>
        /// Performs the fitness calculation using the contract specified by the constructor of an <see cref="Optimiser"/>.
        /// </summary>
        /// <param name="individuals">The individuals which needs their fitness assigned.</param>
        void CalculateAndAssignFitness(IEnumerable<Individual> individuals);
    }
}