using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Optimisers.NelderMead.Test
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
            ind.SetLegality(true);
            ind.FinishEvaluating();
        }
    }
}