using System;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.NelderMead;

namespace PopOptBox.HyperParameterTuning
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