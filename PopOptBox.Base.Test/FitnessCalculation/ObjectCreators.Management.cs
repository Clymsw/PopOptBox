using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.FitnessCalculation;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Base.Test.Helpers
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

        internal static void EvaluateIndividual(Individual ind, double fitness = 2)
        {
            ind.SendForEvaluation();
            ind.SetProperty(Solution_Key, fitness);
            ind.SetSolution(Solution_Key);
        }

        internal static void EvaluateIndividualAsIllegal(Individual ind)
        {
            ind.SendForEvaluation();
            ind.SetIllegal();
        }

        internal static void EvaluateIndividualHasError(Individual ind, Exception ex)
        {
            ind.SendForEvaluation();
            ind.SetIllegal();
            ind.SetProperty(OptimiserPropertyNames.EvaluationError, ex);
        }


        internal static void EvaluateIndividualAndSetFitness(Individual ind, double fitness = 2)
        {
            EvaluateIndividual(ind, fitness);
            ind.SetFitness(fitness);
        }

        internal static Population GetEmptyPopulation(
            int desiredNumber,
            bool constantLengthDecVec = true)
        {
            return new Population(
                desiredNumber,
                constantLengthDv: constantLengthDecVec);
        }
        
        internal class OptimiserMock : Optimiser
        {
            private DecisionVector decisionVector;
        
            public OptimiserMock(
                DecisionVector decisionVector,
                Population initialPopulation,
                IFitnessCalculator fitnessCalculator) 
                : base(initialPopulation, fitnessCalculator)
            {
                this.decisionVector = decisionVector;
                updateDecisionVector(false);
            }

            protected override int AssessFitnessAndDecideFate(IEnumerable<Individual> individuals)
            {
                return Population.Count < 100 ?
                    base.AssessFitnessAndDecideFate(individuals)
                    : 0;
            }

            protected override DecisionVector GetNewDecisionVector()
            {
                updateDecisionVector();
                return decisionVector;
            }

            private void updateDecisionVector(bool add = true)
            {
                var elements = decisionVector.Select(d => (double)d).ToList();
                elements[0] += add ? 0.01 : -0.01;
                var space = decisionVector.GetDecisionSpace();
                decisionVector = DecisionVector.CreateFromArray(
                    space, elements);
            }
        }

        internal class OptimiserBuilderMock : OptimiserBuilder
        {
            public readonly double[] StartingDecVec;
            public const int PopulationSize = 5;
            public const double PenaltyValue = 1000;

            public OptimiserBuilderMock()
            {
                StartingDecVec = new[] {1.2};
            }
            
            public override Optimiser CreateOptimiser()
            {
                return new OptimiserMock(
                    GetDecisionVector(StartingDecVec),
                    GetEmptyPopulation(PopulationSize),
                    CreateFitnessCalculator());
            }

            public override IModel CreateModel()
            {
                return new ModelMock(GetDecisionVector(StartingDecVec), GetConverterMock());
            }

            protected override IFitnessCalculator CreateFitnessCalculator()
            {
                return new FitnessCalculatorSingleObjective(
                    SolutionToFitnessSingleObjective.Minimise, 
                    v => PenaltyValue);
            }

            public IConverter<double> GetConverterMock()
            {
                var converterMock = new Mock<IConverter<double>>();
            
                converterMock.Setup(x => x.ConvertToReality(
                        GetDecisionVector(StartingDecVec)))
                    .Returns(StartingDecVec.ElementAt(0));

                return converterMock.Object;
            }
        }
    }
}