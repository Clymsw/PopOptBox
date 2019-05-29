using Optimisation.Core.Population;

namespace Optimisation.Core.Model
{
    /// <summary>
    /// The Evaluator decides how good a particular Individual (solution) is
    /// </summary>
    /// <typeparam name="TDecVec">Class of decision vector, e.g. int</typeparam>
    public interface IEvaluator
    {
        /// <summary>
        /// Calls evaluation logic on the information placed in the individual by the Model
        /// </summary>
        /// <param name="ind">The individual to evaluate.</param>
        void Evaluate(Individual ind);
    }
}