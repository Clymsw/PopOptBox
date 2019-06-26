using System.Collections.Generic;
using Xunit;
using Optimisation.Base.Management;
using Optimisation.Base.Test.Helpers;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Conversion.Test
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
            var ind = new Individual(DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(1,-1,2), 
                new[] {1.2}));
            
            ind.SetProperty(ObjectCreators.Definition_Key, Test_Solution);
            ind.SendForEvaluation();
            
            evaluatorMock.Evaluate(ind);
            
            Assert.Equal(new[]{Test_Solution}, 
                ind.GetProperty<double[]>(ObjectCreators.Solution_Key));
            Assert.Equal(IndividualStates.Evaluated, ind.State);
            Assert.True(ind.Legal);
        }
    }
}