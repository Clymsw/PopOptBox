using System;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.HyperParameterTuning.SingleObjective.Continuous.NelderMead
{
    public class NelderMeadBuilder : OptimiserBuilder
    {
        private readonly DecisionSpace decisionSpace;

        public NelderMeadBuilder(DecisionSpace decisionSpace)
        {
            this.decisionSpace = decisionSpace;
        }
        
        public static OptimiserBuilder GetBuilder(DecisionSpace problemSpace, double simplexCreationStepSize)
        {
            var builder = new NelderMeadBuilder(problemSpace);
            
            builder.AddHyperParameter(
                new VariableDiscrete(1, int.MaxValue,
                    name: HyperParameterNames.NumberOfDimensions),
                problemSpace.Count);
            
            builder.AddHyperParameter(
                new VariableContinuous(
                    lowerBoundForGeneration: 0.0001,
                    upperBoundForGeneration: 1,
                    name: HyperParameterNames.SimplexStepCreationSize), 
                simplexCreationStepSize);

            return builder;
        }
        
        public override Optimiser CreateOptimiser()
        {
            return new Optimisers.StructuredSearch.NelderMead(
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