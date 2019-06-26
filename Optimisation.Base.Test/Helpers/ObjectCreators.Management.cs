using System;
using System.Collections.Generic;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Test.Helpers
{
    internal static partial class ObjectCreators
    {
        internal const string Solution_Key = "TestSolution";

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
            ind.SetProperty(Solution_Key, new[] {fitness});
            ind.SetSolution(Solution_Key);
            ind.SetScore(sol => sol);
            ind.SetFitness(score => score[0]);
            ind.FinishEvaluating();

            return ind;
        }
        
        internal class OptimiserMock : Optimiser
        {
            private readonly DecisionVector decisionVector;
        
            public OptimiserMock(DecisionVector decisionVector,
                Population initialPopulation, 
                Func<double[], double[]> solutionToScoreDelegate, 
                Func<double[], double> scoreToFitDelegate, 
                Func<double[], double> penaltyDelegate) : base(initialPopulation, solutionToScoreDelegate, scoreToFitDelegate, penaltyDelegate)
            {
                this.decisionVector = decisionVector;
            }

            protected override DecisionVector GetNewDecisionVector()
            {
                return decisionVector;
            }

            protected override bool CheckAcceptable(Individual ind)
            {
                return true;
            }
        }
    }
}