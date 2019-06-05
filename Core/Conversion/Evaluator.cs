using System.Collections.Generic;
using Optimisation.Base.Management;

namespace Optimisation.Base.Conversion
{
    /// <inheritdoc />
    public abstract class Evaluator : IEvaluator
    {
        private readonly string solutionProperty;

        /// <inheritdoc />
        protected Evaluator(string solutionProperty)
        {
            this.solutionProperty = solutionProperty;
        }

        /// <inheritdoc />
        public abstract void Evaluate(Individual ind);

        /// <summary>
        /// Helpful wrapper function for <see cref="Individual"/>.SetSolution()
        /// </summary>
        /// <param name="ind">The individual to set the solution for</param>
        /// <param name="solution">The value.</param>
        protected void SetSingleObjectiveSolution(Individual ind, double solution)
        {
            var solVector = new[]
            {
                solution
            };
            SetSolution(ind, solVector);
        }

        /// <summary>
        /// Unnecessary wrapper function for <see cref="Individual"/>.SetSolution()
        /// </summary>
        /// <param name="ind">The individual to set the solution for</param>
        /// <param name="solutionVector">The values.</param>
        protected void SetMultiObjectiveSolution(Individual ind, IEnumerable<double> solutionVector)
        {
            SetSolution(ind, solutionVector);
        }

        private void SetSolution(Individual ind, IEnumerable<double> solVector)
        {
            ind.SetProperty(solutionProperty, solVector);
            ind.FinishEvaluating();
        }
    }
}