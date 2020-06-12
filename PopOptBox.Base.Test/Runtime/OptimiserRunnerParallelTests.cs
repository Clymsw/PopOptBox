using System;
using System.Linq;
using PopOptBox.Base.PopulationCalculation;
using PopOptBox.Base.Test.Helpers;
using Xunit;

namespace PopOptBox.Base.Runtime.Test
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
                p => p.AbsoluteDecisionVectorConvergence(1),
                p => { });
        }
        
        [Fact]
        public void RunOptimisation_CompletesDueToConvergence_WhenPopulationIsFull()
        {
            runner.Run(newIndividualsPerGeneration: 2);
            
            Assert.Equal(ObjectCreators.OptimiserBuilderMock.PopulationSize, 
                runner.AllEvaluated.Count);
        }
        
        [Fact]
        public void Run_EvaluatorThrowsError_ErrorWrappedAndThrown()
        {
            var errorRunner = new OptimiserRunnerParallel(
                builder,
                new ObjectCreators.EvaluatorWithErrorMock(),
                p => p.AbsoluteDecisionVectorConvergence(1),
                p => { });

            var failed = false;
            try
            {
                errorRunner.Run();
            }
            catch (AggregateException e)
            {
                Assert.IsType<InvalidOperationException>(e.InnerException);
                failed = true;
            }
            Assert.True(failed);
        }
        
        [Fact]
        public void Run_IndividualIsIllegal_AssignsPenaltyValue()
        {
            var illegalRunner = new OptimiserRunnerParallel(
                builder,
                new ObjectCreators.EvaluatorWithErrorMock(false),
                p => p.AbsoluteDecisionVectorConvergence(1),
                p => { });
            illegalRunner.Run();
            Assert.True(illegalRunner.AllEvaluated.All(i => i.Fitness == ObjectCreators.OptimiserBuilderMock.PenaltyValue));
        }
    }
}