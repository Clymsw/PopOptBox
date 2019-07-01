using Optimisation.Base.Management;

namespace Optimisation.Optimisers.NelderMead.Test
{
    public static class Helpers
    {
        public static void EvaluateIndividual(Individual ind)
        {
            ind.SendForEvaluation();
            ind.SetProperty("solution", new[]{1.0});
            ind.SetSolution("solution");
            ind.FinishEvaluating();
        }
    }
}