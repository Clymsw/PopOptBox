using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.StructuredSearch.Simplices
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
        /// <param name="simplex">The simplex.</param>
        /// <returns>A <see cref="DecisionVector"/> for the new vertex.</returns>
        DecisionVector Operate(Simplex simplex);
    }
}
