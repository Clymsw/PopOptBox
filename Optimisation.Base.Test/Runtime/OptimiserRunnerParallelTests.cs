using Optimisation.Base.Helpers;
using Optimisation.Base.Test.Helpers;
using Xunit;

namespace Optimisation.Base.Runtime.Test
{
    public class OptimiserRunnerParallelTests
    {
        private readonly OptimiserRunnerParallel runner;
        private readonly ObjectCreators.OptimiserBuilderMock builder;
        
        public OptimiserRunnerParallelTests()
        {
            builder = new ObjectCreators.OptimiserBuilderMock();
            runner = new OptimiserRunnerParallel(builder, 
                new ObjectCreators.EvaluatorMock(),
                p => p.AbsoluteDecisionVector(1),
                p => { });
        }
        
        [Fact]
        public void RunOptimisation_CompletesDueToConvergence_WhenPopulationIsFull()
        {
            runner.Run();
            
            Assert.Equal(ObjectCreators.OptimiserBuilderMock.PopulationSize, 
                runner.AllEvaluated.Count);
        }
    }
}