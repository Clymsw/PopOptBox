using Optimisation.Base.Conversion;
using Optimisation.Base.Variables;
using System;

namespace Optimisation.Problems.SingleObjective
{
    public abstract class ProblemSingleObjective : Evaluator<DecisionVector>
    {
        private readonly string name;
        protected readonly DecisionVector globalOptimum;

        protected ProblemSingleObjective(
            string name, DecisionVector globalOptimum,
            string definitionKey, string solutionKey) : base(definitionKey, solutionKey)
        {
            if (globalOptimum.Vector.Count < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(globalOptimum),
                    "Number of dimensions must be greater than zero.");
            }
            this.name = name;
            this.globalOptimum = globalOptimum;
        }

        /// <summary>
        /// Gets the information about the location of the global optimum.
        /// </summary>
        /// <returns>A <see cref="DecisionVector"/>.</returns>
        public DecisionVector GetGlobalOptimum()
        {
            return globalOptimum;
        }

        #region Evaluator

        public override bool GetLegality(DecisionVector definition)
        {
            // By definition, if we can construct a decision vector in the same decision space, it is legal
            return definition.GetDecisionSpace() == globalOptimum.GetDecisionSpace()
                ? true
                : false;
        }

        #endregion

        #region ToString
        public override string ToString()
        {
            return $"{name} ({globalOptimum.Vector.Count} dimensions)";
        }
        #endregion
    }
}
