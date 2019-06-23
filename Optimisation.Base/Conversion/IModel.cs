using Optimisation.Base.Management;

namespace Optimisation.Base.Conversion
{
    /// <summary>
    /// The Model manages conversion between optimiser and evaluator
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Returns a new suggested individual based on some real-world logic
        /// </summary>
        /// <returns>An individual</returns>
        Individual GetNewIndividual();
        
        /// <summary>
        /// Looks at an individual and inserts any real world information required for evaluation
        /// </summary>
        /// <param name="ind">The Individual to operate on</param>
        void PrepareForEvaluation(Individual ind);
    }
}