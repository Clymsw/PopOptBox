using Optimisation.Base.Variables;

namespace Optimisation.Problems.SingleObjective.Continuous
{
    public abstract class ProblemSingleObjectiveContinuous : ProblemSingleObjective
    {
        public ProblemSingleObjectiveContinuous(string name, DecisionVector globalOptimum) 
            : base(name, globalOptimum, ContinuousProblemPropertyNames.TheLocation, ContinuousProblemPropertyNames.TheResult)
        {
        }
    }
}
