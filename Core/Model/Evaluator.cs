using System.Collections.Generic;
using Optimisation.Core.Population;

namespace Optimisation.Core.Model
{
    /// <inheritdoc />
    public abstract class Evaluator<TDecVec> : IEvaluator<TDecVec>
    {
        private readonly string solutionProperty;

        /// <inheritdoc />
        protected Evaluator(string solutionProperty)
        {
            this.solutionProperty = solutionProperty;
        }

        /// <inheritdoc />
        public abstract void Evaluate(Individual<TDecVec> ind);

        protected void SetSingleObjectiveSolution(Individual<TDecVec> ind, double solution)
        {
            var solVector = new[]
            {
                solution
            };
            setSolution(ind, solVector);
        }

        protected void SetMultiObjectiveSolution(Individual<TDecVec> ind, IEnumerable<double> solVector)
        {
            setSolution(ind, solVector);
        }

        private void setSolution(Individual<TDecVec> ind, IEnumerable<double> solVector)
        {
            ind.SetProperty(solutionProperty, solVector);
            ind.FinishEvaluating();
        }
    }
}