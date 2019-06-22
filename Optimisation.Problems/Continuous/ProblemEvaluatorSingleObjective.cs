using Optimisation.Base.Conversion;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using System;
using System.Linq;

namespace Optimisation.Problems.Continuous
{
    public abstract class ProblemEvaluatorSingleObjective : Evaluator, IProblemSpace
    {
        private readonly string name;
        private readonly DecisionVector globalOptimum;
        private readonly Model problemModel;

        protected ProblemEvaluatorSingleObjective(string name, DecisionVector globalOptimum) :
            base(ContinuousProblemDefinitions.TheResult)
        {
            if (globalOptimum.Vector.Count < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(globalOptimum),
                    "Number of dimensions must be greater than zero.");
            }
            
            this.name = name;
            this.globalOptimum = globalOptimum;
            problemModel = new ProblemModel();
        }

        #region ToString
        public override string ToString()
        {
            return $"{name} ({globalOptimum.Vector.Count} dimensions)";
        }
        #endregion

        #region Evaluator
        public override void Evaluate(Individual ind)
        {
            problemModel.PrepareForEvaluation(ind);
            var dv = ind.GetProperty<double[]>(ContinuousProblemDefinitions.TheLocation);
            var fitness = GetFitness(dv);
            SetSingleObjectiveSolution(ind, fitness);
        }

        protected abstract double GetFitness(double[] location);
        #endregion

        #region IProblemSpace

        public Individual GetGlobalOptimum()
        {
            var ind = new Individual(globalOptimum);
            problemModel.PrepareForEvaluation(ind);
            Evaluate(ind);
            return ind;
        }

        #endregion
    }
}
