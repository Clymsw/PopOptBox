using Optimisation.Core.Population;

namespace Optimisation.Core.Model
{
    /// <summary>
    /// The Model manages conversion between optimiser and evaluator
    /// </summary>
    /// <typeparam name="TDecVec">Class of decision vector array, e.g. int</typeparam>
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