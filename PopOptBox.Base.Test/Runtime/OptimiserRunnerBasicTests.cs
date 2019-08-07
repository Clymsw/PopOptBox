using PopOptBox.Base.Calculation;
using PopOptBox.Base.Test.Helpers;
using Xunit;

namespace PopOptBox.Base.Runtime.Test
{
    public class OptimiserRunnerBasicTests
    {
        private readonly OptimiserRunnerBasic runner;
        private readonly ObjectCreators.OptimiserBuilderMock builder;
        
        public OptimiserRunnerBasicTests()
        {
            builder = new ObjectCreators.OptimiserBuilderMock();
            runner = new OptimiserRunnerBasic(
                builder, 
                new ObjectCreators.EvaluatorMock(),
                p => p.AbsoluteDecisionVectorConvergence(1),
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