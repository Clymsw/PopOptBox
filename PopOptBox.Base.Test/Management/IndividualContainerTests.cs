using System;
using PopOptBox.Base.Test.Helpers;
using Xunit;

namespace PopOptBox.Base.Management.Test
{
    public class IndividualContainerTests
    {
        private readonly Individual ind;
        private readonly Individual indEqual;
        private readonly Individual indStrictlyBetter;
        private readonly Individual indParetoEqual;
        private readonly Individual indWrong;
        
        public IndividualContainerTests()
        {
            var testVector = new double[] { 0, 2 };
            var dv = ObjectCreators.GetDecisionVector(testVector);
            
            ind = new Individual(dv);
            ind.SendForEvaluation();
            
            indEqual = ind.Clone();
            indStrictlyBetter = ind.Clone();
            indParetoEqual = ind.Clone();
            indWrong = ind.Clone();
            
            var solution1 = new[] {0.2, 5.1, 55};
            ind.SetProperty(ObjectCreators.Solution_Key, solution1);
            indEqual.SetProperty(ObjectCreators.Solution_Key, solution1);
            var solutionStrictlyBetter = new[] {0.1, 5.0, 54.9};
            indStrictlyBetter.SetProperty(ObjectCreators.Solution_Key, solutionStrictlyBetter);
            var solutionParetoEqual = new[] {0.1, 5.2, 55.0};
            indParetoEqual.SetProperty(ObjectCreators.Solution_Key, solutionParetoEqual);
            var solutionWrong = new[] {0.1, 54.9};
            indWrong.SetProperty(ObjectCreators.Solution_Key, solutionWrong);
                
            ind.SetSolution(ObjectCreators.Solution_Key);
            indEqual.SetSolution(ObjectCreators.Solution_Key);
            indStrictlyBetter.SetSolution(ObjectCreators.Solution_Key);
            indParetoEqual.SetSolution(ObjectCreators.Solution_Key);
            indWrong.SetSolution(ObjectCreators.Solution_Key);
        }
        
        [Fact]
        public void ContainedIndividual_IsSameAsUsedInConstructor()
        {
            var container1 = new IndividualContainer(ind);
            
            Assert.Equal(ind, container1.TheIndividual);
        }
        [Fact]
        public void Equals_ContainerWithSameIndividual_True()
        {
            var container1 = new IndividualContainer(ind);
            var container1A = new IndividualContainer(ind);
            
            Assert.Equal(container1, container1A);
        }
        
        [Fact]
        public void Equals_ContainerWithDifferentIndividual_False()
        {
            var container1 = new IndividualContainer(ind);
            var container2 = new IndividualContainer(indStrictlyBetter);
            
            Assert.NotEqual(container1, container2);
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherSolutionVectorIsDifferentLength_Throws()
        {
            var container1 = new IndividualContainer(ind);
            var containerWrong = new IndividualContainer(indWrong);
            
            Assert.Throws<InvalidOperationException>(() => container1.IsDominating(indWrong));
            Assert.Throws<InvalidOperationException>(() => containerWrong.IsDominating(container1.TheIndividual));
        }

        [Fact]
        public void Dominates_IsDominatedBy_OtherIsStrictlyBetter_ReturnsTrue()
        {
            var container1 = new IndividualContainer(ind);
            var container2 = new IndividualContainer(indStrictlyBetter);

            Assert.False(container1.IsDominating(indStrictlyBetter));
            Assert.True(container2.IsDominating(ind));
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherIsEqual_ReturnsFalse()
        {
            var container1 = new IndividualContainer(ind);
            var container2 = new IndividualContainer(indEqual);

            Assert.False(container1.IsDominating(indEqual));
            Assert.False(container2.IsDominating(ind));
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OnSameParetoFront_ReturnsFalse()
        {
            var container1 = new IndividualContainer(ind);
            var container2 = new IndividualContainer(indParetoEqual);

            Assert.False(container1.IsDominating(indParetoEqual));
            Assert.False(container2.IsDominating(ind));
        }
    }
}