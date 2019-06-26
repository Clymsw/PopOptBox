using Optimisation.Base.Conversion;
using Optimisation.Base.Variables;
using System;
using System.Linq;

namespace Optimisation.Problems.Continuous
{
    public abstract class ProblemEvaluatorSingleObjective : Evaluator<double[]>, IProblemEvaluator
    {
        private readonly string name;
        private readonly DecisionVector globalOptimum;

        protected ProblemEvaluatorSingleObjective(string name, DecisionVector globalOptimum) :
            base(ContinuousProblemDefinitions.TheLocation, ContinuousProblemDefinitions.TheResult)
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

        #region ToString
        public override string ToString()
        {
            return $"{name} ({globalOptimum.Vector.Count} dimensions)";
        }
        #endregion

        #region Evaluator

        public override bool GetLegality(double[] definition)
        {
            return globalOptimum.GetDecisionSpace().IsAcceptableDecisionVector(definition.Select(i => (object)i));
        }

        #endregion

        #region IProblemSpace

        public DecisionVector GetGlobalOptimum()
        {
            return globalOptimum;
        }

        #endregion
    }
}
