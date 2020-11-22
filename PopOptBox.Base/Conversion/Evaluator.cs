using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Conversion
{
    /// <summary>
    /// The Evaluator decides how good a particular Individual (solution) is
    /// </summary>
    public abstract class Evaluator<TReality> : IEvaluator
    {
        private readonly string definitionKey;
        private readonly string[] solutionKeys;

        /// <summary>
        /// Constructs the evaluator.
        /// </summary>
        /// <param name="definitionKey">The <see cref="Individual"/> property name for the reality definition.</param>
        /// <param name="solutionKeys">The array of <see cref="Individual"/> property names for the evaluated solution.</param>
        protected Evaluator(string definitionKey, params string[] solutionKeys)
        {
            this.solutionKeys = solutionKeys;
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
                ind.SetIllegal();
                return;
            }
            
            try
            {
                var solution = Evaluate(definition);
                SetSolution(ind, solution);
            }
            catch (Exception e)
            {
                ind.SetIllegal();
                ind.SetProperty(OptimiserPropertyNames.EvaluationError, e);
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
            var vector = solVector as double[] ?? solVector.ToArray();
            
            if (vector.Length != solutionKeys.Length)
                throw new System.InvalidOperationException("Solution Vector and solution key names are different lengths.");

            for (int i = 0; i < solutionKeys.Length; i++)
                ind.SetProperty(solutionKeys[i], vector.ElementAt(i));

            ind.SetSolution(solutionKeys);
        }
    }
}