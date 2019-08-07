using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.StructuredSearch.Test
{
    public static class Helpers
    {
        public static List<Individual> CreateEvaluatedIndividualsFromArray(double[][] testValues)
        {
            var ds = DecisionSpace.CreateForUniformDoubleArray(testValues.ElementAt(0).Length, double.MinValue, double.MaxValue);

            var dvs = testValues.Select(v => DecisionVector.CreateFromArray(ds, v));

            var inds = dvs.Select(v => new Individual(v)).ToList();
            foreach (var ind in inds)
            {
                Helpers.EvaluateIndividual(ind);
            }
            return inds;
        }

        public static void EvaluateIndividual(Individual ind, double value = 1.0)
        {
            ind.SendForEvaluation();
            ind.SetProperty("solution", new[]{value});
            ind.SetSolution("solution");
            ind.SetFitness(s => s[0]);
            ind.SetLegality(true);
            ind.FinishEvaluating();
        }
    }
}