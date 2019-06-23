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
            TReality definition = ind.GetProperty<TReality>(definitionKey);
            var solution = Evaluate(definition);
            SetSolution(ind, solution);
        }

        /// <summary>
        /// Calls evaluation logic on a solution.
        /// </summary>
        /// <param name="solution">The definition of a solution to evaluate</param>
        /// <returns>An array representing how good it is.</returns>
        public abstract IEnumerable<double> Evaluate(TReality solution);

        /// <summary>
        /// Helpful wrapper function for <see cref="Individual"/>.SetSolution()
        /// </summary>
        /// <param name="ind">The individual to set the solution for</param>
        /// <param name="solution">The value.</param>
        //protected void SetSingleObjectiveSolution(Individual ind, double solution)
        //{
        //    var solVector = new[]
        //    {
        //        solution
        //    };
        //    SetSolution(ind, solVector);
        //}

        private void SetSolution(Individual ind, IEnumerable<double> solVector)
        {
            ind.SetProperty(solutionKey, solVector);
            ind.FinishEvaluating();
        }
    }
}