using System;
using Optimisation.Base.Conversion;
using Optimisation.Base.Helpers;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using Optimisation.Optimisers.NelderMead;

namespace Optimisation.HyperParameterTuning
{
    public class NelderMeadBuilder : OptimiserBuilder
    {
        private readonly DecisionSpace decisionSpace;

        public NelderMeadBuilder(DecisionSpace decisionSpace)
        {
            this.decisionSpace = decisionSpace;
        }
        
        public override Optimiser CreateOptimiser()
        {
            return new NelderMead(
                CreateSolutionToFitness(),
                CreatePenalty(),
                CreateModel().GetNewDecisionVector(),
                (double)GetHyperParameterValue(HyperParameterNames.SimplexStepCreationSize));
        }

        public override IModel CreateModel()
        {
            return new NelderMeadForContinuousProblemModel(decisionSpace);
        }

        protected override Func<double[], double> CreateSolutionToFitness()
        {
            return SolutionToFitness.SingleObjectiveMinimise;
        }

        protected override Func<double[], double> CreatePenalty()
        {
            return Penalty.DeathPenalty;
        }
    }
}