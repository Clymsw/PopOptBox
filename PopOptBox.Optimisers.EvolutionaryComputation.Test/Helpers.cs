using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Test
{
    internal static class Helpers
    {
        public static List<Individual> CreateNewIndividualsFromArray(double[][] testValues)
        {
            var ds = DecisionSpace.CreateForUniformDoubleArray(testValues.ElementAt(0).Length, double.MinValue, double.MaxValue);
            var dvs = testValues.Select(v => DecisionVector.CreateFromArray(ds, v));
            return dvs.Select(v => new Individual(v)).ToList();
        }
        
        public static List<Individual> CreateEvaluatedIndividualsFromArray(double[][] testValues, double[] fitness)
        {
            var inds = CreateNewIndividualsFromArray(testValues);
            for (var i = 0; i < inds.Count; i++)
            {
                inds.ElementAt(i).EvaluateIndividual(fitness.ElementAt(i));
            }
            return inds;
        }

        public static List<Individual> CreateFitnessAssessedIndividualsFromArray(double[][] testValues, double[] fitness)
        {
            var inds = CreateNewIndividualsFromArray(testValues);
            for (var i = 0; i < inds.Count; i++)
            {
                inds.ElementAt(i).EvaluateIndividual(fitness.ElementAt(i));
                inds.ElementAt(i).SetFitness(fitness.ElementAt(i));
            }
            return inds;
        }

        public static DecisionVector CreateDecisionVectorWithMixedElements()
        {
            var discreteDs = DecisionSpace.CreateForUniformIntArray(4, int.MinValue, int.MaxValue);
            var continuousDs = DecisionSpace.CreateForUniformDoubleArray(4, double.MinValue, double.MaxValue);
            var variables = discreteDs.ToList();
            variables.AddRange(continuousDs);
            return DecisionVector.CreateFromArray(
                new DecisionSpace(variables),
                new double[8] { 7, 6, 5, 4, 3.1, 2.1, 1.1, 0.0 });
        }
        
        public static void EvaluateIndividual(this Individual ind, double fitness = 1.0)
        {
            ind.SendForEvaluation();
            ind.SetProperty("solution", fitness);
            ind.SetSolution("solution");
            ind.SetLegality(true);
        }
    }
}