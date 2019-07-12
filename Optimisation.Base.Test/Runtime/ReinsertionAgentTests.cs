using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Optimisation.Base.Helpers;
using Optimisation.Base.Management;
using Optimisation.Base.Test.Helpers;
using Xunit;

namespace Optimisation.Base.Runtime.Test
{
    public class ReinsertionAgentTests
    {
        private readonly ReinsertionAgent agent;
        
        public ReinsertionAgentTests()
        {
            var builder = new ObjectCreators.OptimiserBuilderMock();

            agent = new ReinsertionAgent(
                builder.CreateOptimiser(),
                new ObjectCreators.ModelMock(
                    ObjectCreators.GetDecisionVector(builder.DecVec),
                    builder.GetConverterMock()),
                new TimeOutManager(5, TimeSpan.MaxValue),
                p => p.AbsoluteDecisionVectorConvergence(1),
                1)
            {
                SaveAll = true
            };
        }

        [Fact]
        public void IndividualInserted_GetsProcessed()
        {
            var newInd = agent.CreateNewIndividuals(1).ElementAt(0);
            Assert.Equal(IndividualStates.Evaluating, newInd.State);

            var evaluator = new ObjectCreators.EvaluatorMock();
            evaluator.Evaluate(newInd);

            agent.IndividualsForReinsertion.Post(newInd);

            var generatedInd = agent.NewIndividuals.Receive(); // Won't happen without this line.
            
            Assert.True(generatedInd.GetProperty<DateTime>(OptimiserPropertyNames.CreationTime) > 
                        newInd.GetProperty<DateTime>(OptimiserPropertyNames.CreationTime));
            
            Assert.Equal(2, agent.NumberGenerated);
            Assert.True(agent.AllEvaluated.Count == 1);

            var pop = agent.GetCurrentPopulation();
            Assert.True(pop.Count == 1);
            Assert.Equal(newInd, pop[0]);
        }
    }
}