using System;
using System.Linq;
using PopOptBox.Base.PopulationCalculation;
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
        public void Run_CompletesDueToConvergence_WhenPopulationIsFull()
        {
            runner.Run();
            
            Assert.Equal(ObjectCreators.OptimiserBuilderMock.PopulationSize, 
                runner.AllEvaluated.Count);
        }

        [Fact]
        public void Run_EvaluatorThrowsError_ErrorWrappedAndThrown()
        {
            var errorRunner = new OptimiserRunnerBasic(
                builder,
                new ObjectCreators.EvaluatorWithErrorMock(),
                p => p.AbsoluteDecisionVectorConvergence(1),
                p => { });
            Assert.Throws<InvalidOperationException>(() => errorRunner.Run());
        }
        
        [Fact]
        public void Run_IndividualIsIllegal_AssignsPenaltyValue()
        {
            var illegalRunner = new OptimiserRunnerBasic(
                builder,
                new ObjectCreators.EvaluatorWithErrorMock(false),
                p => p.AbsoluteDecisionVectorConvergence(1),
                p => { });
            illegalRunner.Run();
            Assert.True(illegalRunner.AllEvaluated.All(i => i.Fitness == ObjectCreators.OptimiserBuilderMock.PenaltyValue));
        }
    }
}