using Xunit;
using PopOptBox.Base.Management;
using PopOptBox.Base.Test.Helpers;

namespace PopOptBox.Base.Conversion.Test
{
    public class EvaluatorTests
    {
        private readonly Evaluator<double> evaluatorMock;
        private const double Test_Solution = 4.0;
        
        public EvaluatorTests()
        {
            evaluatorMock = new ObjectCreators.EvaluatorMock();
        }

        [Fact]
        public void EvaluatesCorrectly()
        {
            var ind = ObjectCreators.GetIndividual(new[] {1.2});
            
            ind.SetProperty(ObjectCreators.Definition_Key, Test_Solution);
            ind.SendForEvaluation();
            
            evaluatorMock.Evaluate(ind);
            
            Assert.Equal(Test_Solution, 
                ind.GetProperty<double>(ObjectCreators.Solution_Key));
            Assert.Equal(IndividualState.Evaluated, ind.State);
            Assert.True(ind.Legal);
            Assert.Equal(new[]{Test_Solution}, ind.SolutionVector);
        }
    }
}