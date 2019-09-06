using System.Collections.Generic;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Calculation
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
        void PerformSort(IEnumerable<Individual> individuals);
    }
}