using PopOptBox.Base.Conversion;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.StructuredSearch;

namespace PopOptBox.HyperParameterTuning.SingleObjective.Continuous.NelderMead
{
    public class NelderMeadBuilder : OptimiserBuilder
    {
        private readonly DecisionSpace decisionSpace;

        public NelderMeadBuilder(DecisionSpace decisionSpace, HyperParameterManager hyperParameters)
        {
            this.decisionSpace = decisionSpace;
            HyperParameters.AddFromExistingHyperParameterSet(hyperParameters);
        }
        
        public static OptimiserBuilder GetBuilder(
            DecisionSpace problemSpace, 
            double? simplexCreationStepSize = null)
        {
            var hyps = NelderMeadHyperParameters.GetDefaultHyperParameters();
            if (simplexCreationStepSize != null)
                hyps.UpdateHyperParameterValue(
                    NelderMeadHyperParameters.Simplex_Creation_Step_Size,
                    simplexCreationStepSize);
            
            return new NelderMeadBuilder(problemSpace, hyps);
        }
        
        public override Optimiser CreateOptimiser()
        {
            return new Optimisers.StructuredSearch.NelderMead(
                CreateFitnessCalculator(),
                CreateModel().GetNewDecisionVector(),
                HyperParameters);
        }

        public override IModel CreateModel()
        {
            return new ContinuousProblemModel(decisionSpace);
        }

        protected override IFitnessCalculator CreateFitnessCalculator()
        {
            return new FitnessCalculatorSingleObjective(
                SolutionToFitnessSingleObjective.Minimise,
                Penalty.DeathPenalty);
        }
    }
}