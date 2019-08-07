using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Test
{
    public static class Helpers
    {
        public static List<Individual> CreateEvaluatedIndividualsFromArray(double[][] testValues, double[] fitness)
        {
            var ds = DecisionSpace.CreateForUniformDoubleArray(testValues.ElementAt(0).Length, double.MinValue, double.MaxValue);

            var dvs = testValues.Select(v => DecisionVector.CreateFromArray(ds, v));

            var inds = dvs.Select(v => new Individual(v)).ToList();
            for (var i = 0; i < inds.Count; i++)
            {
                inds.ElementAt(i).EvaluateIndividual(fitness.ElementAt(i));
            }
            return inds;
        }

        public static void EvaluateIndividual(this Individual ind, double fitness = 1.0)
        {
            ind.SendForEvaluation();
            ind.SetProperty("solution", new[] { fitness });
            ind.SetSolution("solution");
            ind.SetLegality(true);
            ind.FinishEvaluating();

            ind.SetFitness(SolutionToFitness.SingleObjectiveMinimise);
        }
    }
}