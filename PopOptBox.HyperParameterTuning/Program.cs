using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.PopulationCalculation;
using PopOptBox.Base.Management;
using PopOptBox.Base.MultiObjectiveCalculation;
using PopOptBox.Base.Runtime;
using PopOptBox.Base.Variables;
using PopOptBox.HyperParameterTuning.MultipleObjective.Continuous.EvolutionaryAlgorithm;
using PopOptBox.HyperParameterTuning.SingleObjective.Continuous.EvolutionaryAlgorithm;
using PopOptBox.HyperParameterTuning.SingleObjective.Continuous.NelderMead;
using PopOptBox.Problems.Performance;

namespace PopOptBox.HyperParameterTuning
{
    class Program
    {
        private const double Convergence_Tolerance = 0.00001;
        private const int Number_Of_Restarts = 100;
        private const double Fitness_Tolerance = 0.01;
        
        private const bool RunSingleObjective = false;
        private const Options.ProblemsSingleObjectiveContinuousAvailable SingleObjectiveProblemToUse =
            Options.ProblemsSingleObjectiveContinuousAvailable.Ellipsoidal;
        private const Options.ProblemsMultipleObjectiveContinuousAvailable MultipleObjectiveProblemToUse =
            Options.ProblemsMultipleObjectiveContinuousAvailable.Zdt1;
        
        private const int Number_Of_Dimensions = 6;
        
        private const Options.OptimisersAvailable OptimiserToUse = 
            Options.OptimisersAvailable.EvolutionaryAlgorithm;

        private const int Number_Of_New_Individuals_Per_Generation = 10;
        private const double Nelder_Mead_Simplex_Creation_Step_Size = 0.5;
        
        static void Main(string[] args)
        {
            OptimiserBuilder builder;

            if (RunSingleObjective)
            {
                var problem = Options.GetSingleObjectiveContinuousProblem(SingleObjectiveProblemToUse,
                                    Number_Of_Dimensions);

                switch (OptimiserToUse)
                {
                    case Options.OptimisersAvailable.NelderMead:
                        builder = NelderMeadBuilder.GetBuilder(
                            problem.GetGlobalOptimum().GetDecisionSpace(),
                            Nelder_Mead_Simplex_Creation_Step_Size);
                        break;

                    case Options.OptimisersAvailable.EvolutionaryAlgorithm:
                        builder = EvolutionaryAlgorithmBuilderContinuousSO.GetBuilder(
                            problem.GetGlobalOptimum().GetDecisionSpace(),
                            AvailableOperators.ParentSelector.Tournament,
                            AvailableOperators.RecombinationOperator.Sbx,
                            AvailableOperators.MutationOperators.AddRandomNumber,
                            AvailableOperators.ReinsertionOperators.ReplaceRandom,
                            150);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var runner = new ProblemPerformanceAssessor<double>(builder, problem,
                            p => p.AbsoluteDecisionVectorConvergence(Convergence_Tolerance));
                
                var results = runner.RunAssessment(
                Number_Of_Restarts,
                1000,
                r => Console.Write("."),
                i =>
                {
                    Console.WriteLine(); 
                    Console.WriteLine($"Optimisation {i}/{Number_Of_Restarts} complete.");
                    Console.WriteLine(); 
                },
                timeOutEvaluations: Math.Max(5000, (int)Math.Pow(Number_Of_Dimensions, 3.0) * 20),
                numberOfNewIndividualsPerGeneration: OptimiserToUse == Options.OptimisersAvailable.NelderMead ? 1 : Number_Of_New_Individuals_Per_Generation);

                printSingleObjectiveResults(results);
            }
            else
            {
                var problem = Options.GetMultipleObjectiveContinuousProblem(MultipleObjectiveProblemToUse);

                builder = EvolutionaryAlgorithmBuilderContinuousMO.GetBuilder(
                    problem.GetOptimalParetoFront(1).First().GetDecisionSpace());
                
                var optimiserRunner = new OptimiserRunnerBasic(builder, problem, 
                    pop => false, r => Console.Write("."));

                optimiserRunner.Run(
                    reportingFrequency: 1000,
                    timeOutEvaluations: 10000, 
                    timeOutDuration: null,
                    newIndividualsPerGeneration: Number_Of_New_Individuals_Per_Generation);

                new FastNonDominatedSort().PerformSort(optimiserRunner.FinalPopulation, new[] {true, true});
                var finalFrontier = optimiserRunner.FinalPopulation.ParetoFront(1);
                var comparison = problem.GetOptimalParetoFront(finalFrontier.Length);

            }
        }

        private static void printSingleObjectiveResults(List<ProblemPerformanceSingleObjective> results)
        {
            Console.WriteLine();

            Console.WriteLine("Global optimum location: " +
                $"{results.First().GlobalOptimumLocation}");
            Console.WriteLine("Global optimum solution: " + 
                $"{results.First().GlobalOptimumSolution.ElementAt(0).ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}");

            Console.WriteLine("Best location found: " +
                $"{results.OrderBy(r => r.BestFitness).First().BestLocation}");
            Console.WriteLine("Best solution found: " + 
                $"{results.OrderBy(r => r.BestFitness).First().BestSolution.ElementAt(0).ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}");

            Console.WriteLine("Mean number of evaluations required to find best solution: " +
                $"{results.Average(r => r.EvaluationsToFindBest).ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}");

            Console.WriteLine("Mean solution found: " +
                $"{results.Average(r => r.BestSolution.ElementAt(0)).ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}");

            var threshold = results.ElementAt(0).GlobalOptimumSolution.ElementAt(0) + Fitness_Tolerance;
            var pctNearOptimum = (double)results
                .Count(r => r.BestSolution.ElementAt(0) <= threshold) 
                / results.Count * 100;
            Console.WriteLine("Proportion of solutions below " + 
                $"{threshold.ToString("F", System.Globalization.CultureInfo.InvariantCulture)}: " +
                $"{pctNearOptimum.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}%");
            
            Console.WriteLine("Mean number of evaluations required to converge: " +
                $"{results.Average(r => r.EvaluationsToConverge).ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}");
            Console.WriteLine("Mean time required to converge: " +
                $"{results.Average(r => r.TimeToConverge.TotalSeconds).ToString("F2", System.Globalization.CultureInfo.InvariantCulture)} seconds");
        }
    }
}