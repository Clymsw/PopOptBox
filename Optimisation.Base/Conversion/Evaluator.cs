using System.Collections.Generic;
using Optimisation.Base.Management;

namespace Optimisation.Base.Conversion
{
    /// <inheritdoc />
    public abstract class Evaluator<TReality> : IEvaluator
    {
        private readonly string definitionKey;
        private readonly string solutionKey;

        /// <inheritdoc />
        public Evaluator(string definitionKey, string solutionKey)
        {
            this.solutionKey = solutionKey;
            this.definitionKey = definitionKey;
        }

        /// <inheritdoc />
        public virtual void Evaluate(Individual ind)
        {
            if (ind.State != IndividualStates.Evaluating)
                throw new System.InvalidOperationException("Individual is not ready for evaluation.");

            var definition = ind.GetProperty<TReality>(definitionKey);
            if (!GetLegality(definition))
            {
                ind.SetLegality(false);
            }
            else
            {
                var solution = Evaluate(definition);
                setSolution(ind, solution);
                ind.SetLegality(true);
            }
        }

        /// <summary>
        /// Calls evaluation logic on a solution.
        /// </summary>
        /// <param name="definition">The definition of a object to evaluate</param>
        /// <returns>An array representing how good it is.</returns>
        public abstract IEnumerable<double> Evaluate(TReality definition);

        /// <summary>
        /// Determines if a solution is allowed or not, according to real-world logic.
        /// </summary>
        /// <param name="definition">The definition of a object to evaluate</param>
        /// <returns><see langword="true"/> if legal</returns>
        public abstract bool GetLegality(TReality definition);

        private void setSolution(Individual ind, IEnumerable<double> solVector)
        {
            ind.SetProperty(solutionKey, solVector);
            ind.SetSolution(solutionKey);
            ind.FinishEvaluating();
        }
    }
}