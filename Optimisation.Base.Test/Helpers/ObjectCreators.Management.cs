using System.Collections.Generic;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Test.Helpers
{
    internal static partial class ObjectCreators
    {
        internal const string SolutionKey = "solution";
        
        internal static Individual GetIndividual(IEnumerable<int> vector)
        {
            var dv = GetDecisionVector(vector);
            return new Individual(dv);
        }
        
        internal static Individual GetIndividual(IEnumerable<double> vector)
        {
            var dv = GetDecisionVector(vector);
            return new Individual(dv);
        }

        internal static Individual EvaluateIndividual(Individual ind, double fitness = 2)
        {
            ind.SendForEvaluation();
            ind.SetProperty(SolutionKey, new[] {fitness});
            ind.SetSolution(SolutionKey);
            ind.SetScore(sol => sol);
            ind.SetFitness(score => score[0]);
            ind.FinishEvaluating();

            return ind;
        }
    }
}