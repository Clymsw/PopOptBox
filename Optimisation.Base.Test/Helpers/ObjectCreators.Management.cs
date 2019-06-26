using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Optimisation.Base.Conversion;
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

        internal static Population GetEmptyPopulation(int desiredNumber)
        {
            return new Population(desiredNumber);
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

            protected override bool ReInsert(Individual ind)
            {
                return Population.Count < 100 && base.ReInsert(ind);
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

        internal class OptimiserBuilderMock : OptimiserBuilder
        {
            public readonly double[] DecVec;
            public const int PopulationSize = 5;
            public const double PenaltyValue = 1000;

            public OptimiserBuilderMock()
            {
                DecVec = new[] {1.2};
            }
            
            public override Optimiser CreateOptimiser()
            {
                return new OptimiserMock(
                    GetDecisionVector(DecVec),
                    GetEmptyPopulation(PopulationSize), 
                    CreateMultiObjectiveScore(),
                    CreateObjective(),
                    CreatePenalty());
            }

            public override IModel CreateModel()
            {
                return new ModelMock(GetDecisionVector(DecVec), GetConverterMock());
            }

            protected override Func<double[], double> CreateObjective()
            {
                return v => v.ElementAt(0);
            }

            protected override Func<double[], double[]> CreateMultiObjectiveScore()
            {
                return v => v;
            }

            protected override Func<double[], double> CreatePenalty()
            {
                return v => PenaltyValue;
            }

            public IConverter<double> GetConverterMock()
            {
                var converterMock = new Mock<IConverter<double>>();
            
                converterMock.Setup(x => x.ConvertToReality(
                        GetDecisionVector(DecVec)))
                    .Returns(DecVec.ElementAt(0));

                return converterMock.Object;
            }
        }
    }
}