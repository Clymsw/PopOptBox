using System.Collections.Generic;
using Optimisation.Base.Management;

namespace Optimisation.Base.Conversion
{
    /// <summary>
    /// The Evaluator decides how good a particular Individual (solution) is
    /// </summary>
    public abstract class Evaluator<TReality> : IEvaluator
    {
        private readonly string definitionKey;
        private readonly string solutionKey;

        /// <summary>
        /// Constructs the evaluator.
        /// </summary>
        /// <param name="definitionKey">The <see cref="Individual"/> property name for the reality definition.</param>
        /// <param name="solutionKey">The <see cref="Individual"/> property name for the evaluated solution.</param>
        public Evaluator(string definitionKey, string solutionKey)
        {
            this.solutionKey = solutionKey;
            this.definitionKey = definitionKey;
        }

        /// <summary>
        /// Calls evaluation logic on the information placed in the individual by the Model
        /// </summary>
        /// <param name="ind">The individual to evaluate.</param>
        /// <exception cref="System.InvalidOperationException">Thrown when the individual is not in the correct <see cref="IndividualState"/>.</exception>
        public virtual void Evaluate(Individual ind)
        {
            if (ind.State != IndividualState.Evaluating)
                throw new System.InvalidOperationException("Individual is not ready for evaluation.");

            var definition = ind.GetProperty<TReality>(definitionKey);
            if (!GetLegality(definition))
            {
                ind.SetLegality(false);
            }
            else
            {
                var solution = Evaluate(definition);
                SetSolution(ind, solution);
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

        private void SetSolution(Individual ind, IEnumerable<double> solVector)
        {
            ind.SetProperty(solutionKey, solVector);
            ind.SetSolution(solutionKey);
            ind.FinishEvaluating();
        }
    }
}