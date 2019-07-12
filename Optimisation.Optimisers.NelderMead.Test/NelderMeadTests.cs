using System;
using System.Linq;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Optimisers.NelderMead.Test
{
    public class NelderMeadTests
    {
        #region Non-test functions and fields

        private readonly NelderMead optimiser;
        private const int Number_Of_Dimensions = 2;
        private const double Step_Size = 1;
        private const double Initial_Fitness = 3.0;
        private const double Fitness_Step = 0.1;
            
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

        private double PerformInitialSetup()
        {
            // Evaluate initial simplex
            var fitness = Initial_Fitness;

            for (var i = 0; i <= Number_Of_Dimensions; i++)
            {
                fitness -= Fitness_Step;
                var newInd = optimiser.GetNextToEvaluate(1).ElementAt(0);
                Helpers.EvaluateIndividual(newInd, fitness);
                optimiser.ReInsert(new[] { newInd });
            }

            return fitness;
        }

        #endregion

        #region Optimiser

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

        #endregion

        #region Lagarias et al. expected simplex search behaviour

        [Fact]
        public void Reinsertion_StartsWithReflection()
        {
            PerformInitialSetup();
            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.R);
        }

        [Fact]
        public void Reinsertion_ReflectionIsBest_TriesExpansion()
        {
            var fitness = PerformInitialSetup();
            
            // Reflection vertex
            var ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness - 0.1);
            optimiser.ReInsert(new[] {ind});
            
            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.E);
        }

        [Fact]
        public void Reinsertion_ReflectionIsEqualBest_TriesExpansion()
        {
            //TODO: Check this is expected behaviour
            var fitness = PerformInitialSetup();

            // Reflection vertex
            var ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness);
            optimiser.ReInsert(new[] { ind });

            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.E);
        }

        [Fact]
        public void Reinsertion_ReflectionIsBetterThanExpansion_ChoosesReflection()
        {
            var fitness = PerformInitialSetup();
            
            // Reflection vertex
            var ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness - Fitness_Step);
            optimiser.ReInsert(new[] {ind});
            
            // Expansion vertex
            ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness + Fitness_Step/10);
            optimiser.ReInsert(new[] {ind});
            
            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.R);
            Assert.True(optimiser.LastStep == NelderMeadSteps.reR);
        }

        [Fact]
        public void Reinsertion_ReflectionIsEqualToExpansion_ChoosesReflection()
        {
            // TODO - Check this is expected behaviour
            var fitness = PerformInitialSetup();

            // Reflection vertex
            var ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness - Fitness_Step);
            optimiser.ReInsert(new[] { ind });

            // Expansion vertex
            ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness - Fitness_Step);
            optimiser.ReInsert(new[] { ind });

            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.R);
            Assert.True(optimiser.LastStep == NelderMeadSteps.reE);
        }

        [Fact]
        public void Reinsertion_ReflectionIsWorseThanExpansion_ChoosesExpansion()
        {
            var fitness = PerformInitialSetup();
            
            // Reflection vertex
            var ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness - Fitness_Step);
            optimiser.ReInsert(new[] {ind});
            
            // Expansion vertex
            ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness - (2 * Fitness_Step));
            optimiser.ReInsert(new[] {ind});
            
            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.R);
            Assert.True(optimiser.LastStep == NelderMeadSteps.reE);
        }

        [Fact]
        public void Reinsertion_ReflectionIsMiddling_ChoosesReflection()
        {
            var fitness = PerformInitialSetup();

            // Reflection vertex
            var ind = optimiser.GetNextToEvaluate(1).ElementAt(0);
            Helpers.EvaluateIndividual(ind, fitness + Fitness_Step/10);
            optimiser.ReInsert(new[] { ind });

            Assert.True(optimiser.CurrentOperation == NelderMeadSimplexOperations.R);
            Assert.True(optimiser.LastStep == NelderMeadSteps.rR);
        }

        #endregion

        
    }
}