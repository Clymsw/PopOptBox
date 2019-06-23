using Optimisation.Base.Management;
using System.Collections.Generic;

namespace Optimisation.Base.Conversion
{
    /// <summary>
    /// The Evaluator decides how good a particular Individual (solution) is
    /// </summary>
    public interface IEvaluator
    {
        /// <summary>
        /// Calls evaluation logic on the information placed in the individual by the Model
        /// </summary>
        /// <param name="ind">The individual to evaluate.</param>
        void Evaluate(Individual ind);
    }
}