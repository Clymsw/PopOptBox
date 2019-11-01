using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using PopOptBox.Base.PopulationCalculation;
using PopOptBox.Base.Management;
using PopOptBox.Base.Test.Helpers;
using Xunit;

namespace PopOptBox.Base.Runtime.Test
{
    public class ReinsertionAgentTests
    {
        private readonly ReinsertionAgent agent;
        private const int NumberOfNewIndividuals = 2;
        
        public ReinsertionAgentTests()
        {
            var builder = new ObjectCreators.OptimiserBuilderMock();

            agent = new ReinsertionAgent(
                builder.CreateOptimiser(),
                new ObjectCreators.ModelMock(
                    ObjectCreators.GetDecisionVector(builder.StartingDecVec),
                    builder.GetConverterMock()),
                new TimeOutManager(5, TimeSpan.MaxValue),
                p => p.AbsoluteDecisionVectorConvergence(1),
                1, NumberOfNewIndividuals)
            {
                SaveAll = true
            };
        }

        [Fact]
        public void IndividualInserted_GetsProcessed()
        {
            var newInd = agent.CreateNewIndividuals(1).ElementAt(0);
            Assert.Equal(IndividualState.Evaluating, newInd.State);

            var evaluator = new ObjectCreators.EvaluatorMock();
            evaluator.Evaluate(newInd);

            agent.IndividualsForReinsertion.Post(newInd);

            var generatedInds = new List<Individual>();
            for (var i = 0; i < NumberOfNewIndividuals; i++)
            {
                generatedInds.Add(agent.NewIndividuals.Receive()); // Won't happen without this line.
            }
            
            Assert.Equal(NumberOfNewIndividuals, generatedInds.Count);
            Assert.True(generatedInds.ElementAt(0).GetProperty<DateTime>(OptimiserPropertyNames.CreationTime) > 
                        newInd.GetProperty<DateTime>(OptimiserPropertyNames.CreationTime));
            
            Assert.Equal(NumberOfNewIndividuals + 1, agent.NumberGenerated);
            Assert.True(agent.AllEvaluated.Count == 1);

            var pop = agent.GetCurrentPopulation();
            Assert.True(pop.Count == 1);
            Assert.Equal(newInd, pop[0]);
        }
    }
}