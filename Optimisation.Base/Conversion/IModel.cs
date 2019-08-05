using Optimisation.Base.Management;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Conversion
{
    /// <summary>
    /// The Model manages conversion between optimiser and evaluator.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Returns a new suggested Decision Vector, based on some real-world logic.
        /// </summary>
        /// <returns>A Decision Vector.</returns>
        DecisionVector GetNewDecisionVector();
        
        /// <summary>
        /// Looks at an individual and inserts any real world information required for evaluation.
        /// </summary>
        /// <param name="ind">The Individual to operate on.</param>
        void PrepareForEvaluation(Individual ind);
    }
}