using Optimisation.Base.Variables;
using System.Collections.Generic;

namespace Optimisation.Optimisers.NelderMead.Simplex
{
    /// <summary>
    /// A simplex operator creates a new vertex based on an existing simplex,
    /// using deterministic logic.
    /// </summary>
    public interface ISimplexOperator
    {
        /// <summary>
        /// Returns a new vertex location based on an existing vertex.
        /// </summary>
        /// <param name="orderedVertices">A list of vertices comprising the simplex, in order of fitness (first is best).</param>
        /// <returns>A <see cref="DecisionVector"/> for the new vertex.</returns>
        DecisionVector Operate(IEnumerable<DecisionVector> orderedVertices);
    }
}
