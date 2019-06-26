using System.Collections.Generic;
using Xunit;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Conversion.Test
{
    public class EvaluatorTests
    {
        private readonly Evaluator<double> evaluatorMock;
        private const string Definition_Key = "TestDefinition";
        private const string Solution_Key = "TestSolution";
        private const double Test_Solution = 4.0;
        
        public EvaluatorTests()
        {
            evaluatorMock = new EvaluatorMock(Definition_Key, Solution_Key);
        }

        [Fact]
        public void EvaluatesCorrectly()
        {
            var ind = new Individual(DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(1,-1,2), 
                new[] {1.2}));
            
            ind.SetProperty(Definition_Key, Test_Solution);
            ind.SendForEvaluation();
            
            evaluatorMock.Evaluate(ind);
            
            Assert.Equal(new[]{Test_Solution}, ind.GetProperty<double[]>(Solution_Key));
            Assert.Equal(IndividualStates.Evaluated, ind.State);
            Assert.True(ind.Legal);
        }
    }

    internal class EvaluatorMock : Evaluator<double>
    {
        public EvaluatorMock(string defKey, string solKey) : base(defKey, solKey)
        {
        }

        public override IEnumerable<double> Evaluate(double definition)
        {
            return new[] { definition };
        }

        public override bool GetLegality(double definition)
        {
            return true;
        }
    }
}