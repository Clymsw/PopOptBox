using System;
using System.Linq;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Optimisers.NelderMead.Test
{
    public class NelderMeadTests
    {
        private readonly NelderMead optimiser;
        private const int Number_Of_Dimensions = 3;
        private const double Step_Size = 1;
            
        public NelderMeadTests()
        {
            optimiser = new NelderMead(
                v => v,
                v => v.ElementAt(0),
                v => 1000.0,
                DecisionVector.CreateFromArray(
                    DecisionSpace.CreateForUniformDoubleArray(Number_Of_Dimensions, double.MinValue, double.MaxValue),
                    Enumerable.Repeat(0.0, Number_Of_Dimensions)),
                Step_Size);
        }

        [Fact]
        public void Reinsertion_UnexpectedIndividual_ThrowsError()
        {
            var wrongIndividual = new Individual(
                DecisionVector.CreateFromArray(
                    DecisionSpace.CreateForUniformIntArray(Number_Of_Dimensions + 1, 
                        0, int.MaxValue),
                    Enumerable.Repeat<int>(5, Number_Of_Dimensions)));
            Helpers.EvaluateIndividual(wrongIndividual);

            // Should it throw an error?
            Assert.Throws<ArgumentException>(() => optimiser.ReInsert(new[] {wrongIndividual}));
        }
        
        [Fact]
        public void Reinsertion_EvaluatedIndividual_AddedToPopulation()
        {
            var newInd = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(newInd);

            Assert.Empty(optimiser.Population);
            
            var numReinserted = optimiser.ReInsert(new[] {newInd});
            
            Assert.Equal(1, numReinserted);
            Assert.True(optimiser.Population.Count == 1);
            
            var reinsertionTime = newInd.GetProperty<DateTime>(OptimiserDefinitions.ReinsertionTime);
            Assert.True(reinsertionTime < DateTime.Now);
        }
    }
}