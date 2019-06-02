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

        protected void SetSingleObjectiveSolution(Individual ind, double solution)
        {
            var solVector = new[]
            {
                solution
            };
            setSolution(ind, solVector);
        }

        protected void SetMultiObjectiveSolution(Individual ind, IEnumerable<double> solVector)
        {
            setSolution(ind, solVector);
        }

        private void setSolution(Individual ind, IEnumerable<double> solVector)
        {
            ind.SetProperty(solutionProperty, solVector);
            ind.FinishEvaluating();
        }
    }
}