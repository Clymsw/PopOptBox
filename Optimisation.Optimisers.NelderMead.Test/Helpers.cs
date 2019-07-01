using Optimisation.Base.Management;

namespace Optimisation.Optimisers.NelderMead.Test
{
    public static class Helpers
    {
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