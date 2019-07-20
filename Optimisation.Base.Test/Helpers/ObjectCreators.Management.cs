using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Optimisation.Base.Conversion;
using Optimisation.Base.Helpers;
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
                Func<double[], double[]> solutionToScore, 
                Func<double[], double> scoreToFitness, 
                Func<double[], double> penalty) : base(initialPopulation, solutionToScore, scoreToFitness, penalty)
            {
                this.decisionVector = decisionVector;
            }

            protected override bool ReInsert(Individual individual)
            {
                return Population.Count < 100 && base.ReInsert(individual);
            }

            protected override DecisionVector GetNewDecisionVector()
            {
                return decisionVector;
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
                    CreateSolutionToScore(),
                    CreateScoreToFitness(),
                    CreatePenalty());
            }

            public override IModel CreateModel()
            {
                return new ModelMock(GetDecisionVector(DecVec), GetConverterMock());
            }

            protected override Func<double[], double> CreateScoreToFitness()
            {
                return ScoreToFitness.SingleObjectiveMinimise;
            }

            protected override Func<double[], double[]> CreateSolutionToScore()
            {
                return SolutionToScore.SingleObjective;
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