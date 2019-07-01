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
        private const int Number_Of_Dimensions = 2;
        private const double Step_Size = 1;
        private const double Initial_Fitness = 3.0;
            
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

        [Fact]
        public void Reinsertion_ExpectStartWithReflection_DoesReflection()
        {
            PerformInitialSetup();
            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.R);
        }

        [Fact]
        public void Reinsertion_ReflectionIsBest_DoesExpansion()
        {
            var fitness = PerformInitialSetup();
            
            var ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness - 0.1);
            optimiser.ReInsert(new[] {ind});
            
            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.E);
        }
        
        [Fact]
        public void Reinsertion_ReflectionIsBetterThanExpansion_ChoosesReflection()
        {
            var fitness = PerformInitialSetup();
            
            var ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness - 0.1);
            optimiser.ReInsert(new[] {ind});
            
            ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness + 0.01);
            optimiser.ReInsert(new[] {ind});
            
            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.R);
            Assert.True(optimiser.LastStep == NelderMeadSteps.ReR);
        }
        
        [Fact]
        public void Reinsertion_ReflectionIsWorseThanExpansion_ChoosesExpansion()
        {
            var fitness = PerformInitialSetup();
            
            var ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness - 0.1);
            optimiser.ReInsert(new[] {ind});
            
            ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness - 0.1);
            optimiser.ReInsert(new[] {ind});
            
            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.R);
            Assert.True(optimiser.LastStep == NelderMeadSteps.ReE);
        }

        private double PerformInitialSetup()
        {
            // Evaluate initial simplex
            var fitness = Initial_Fitness;
            
            for (var i = 0; i <= Number_Of_Dimensions; i++)
            {
                fitness -= 0.1;
                var newInd = optimiser.GetNextToEvaluate(1).ElementAt(0);
                Helpers.EvaluateIndividual(newInd, fitness);
                optimiser.ReInsert(new[] {newInd});
            }

            return fitness;
        }
    }
}