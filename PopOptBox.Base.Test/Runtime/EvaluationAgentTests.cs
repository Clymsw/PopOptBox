using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Xunit;
using PopOptBox.Base.Test.Helpers;

namespace PopOptBox.Base.Runtime.Test
{
    public class EvaluationAgentTests
    {
        private readonly EvaluationAgent agent;
        private const double Test_Solution = 4.0;
        
        public EvaluationAgentTests()
        {
            agent = new EvaluationAgent(new ObjectCreators.EvaluatorMock(), CancellationToken.None);
        }
        
        [Fact]
        public void IndividualIsEvaluated()
        {
            var newInd = ObjectCreators.GetIndividual(new[] {1.7});

            newInd.SetProperty(ObjectCreators.Definition_Key, Test_Solution);
            newInd.SendForEvaluation();
            
            agent.IndividualsForEvaluation.Post(newInd);
            agent.EvaluatedIndividuals.Receive(); // Won't happen without this line.
            
            Assert.Equal(Test_Solution, newInd.SolutionVector.ElementAt(0));
        }
    }
}