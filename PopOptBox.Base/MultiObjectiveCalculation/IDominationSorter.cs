using System.Collections.Generic;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.MultiObjectiveCalculation
{
    /// <summary>
    /// Interface for sorting algorithms to perform non-domination sorting and calculate Pareto Fronts.
    /// </summary>
    public interface IDominationSorter
    {
        /// <summary>
        /// Performs sorting. Makes use of:
        /// - <see cref="OptimiserPropertyNames.Dominating"/>
        /// - <see cref="OptimiserPropertyNames.DominatedBy"/>
        /// - <see cref="OptimiserPropertyNames.ParetoFront"/>
        /// </summary>
        /// <param name="individuals">All individuals to consider while calculating Pareto Fronts and domination.</param>
        /// <param name="minimise">An array of the same length as the Solution Vectors, <see langword="true"/> if that objective is to be minimised.</param>
        void PerformSort(IEnumerable<Individual> individuals, bool[] minimise);
    }
}