using System;
using System.Linq;
using PopOptBox.Base.Calculation;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;
using PopOptBox.Problems.HyperparameterOptimisation;
using PopOptBox.Problems.SingleObjective.Continuous;

namespace PopOptBox.HyperParameterTuning
{
    class Program
    {
        private const int Number_Of_Dimensions = 10;
        private const double Simplex_Creation_Step_Size = 0.5;
        private const double Convergence_Tolerance = 0.00001;
        private const int Number_Of_Restarts = 1000;
        private const double Fitness_Tolerance = 0.01;
        
        private static ProblemSingleObjectiveContinuous GetEvaluator()
        {
            //return new Ellipsoidal(Number_Of_Dimensions);
            //return new Schwefel(Number_Of_Dimensions);

            return new Rosenbrock(Number_Of_Dimensions);
            //return new StyblinskiTang(Number_Of_Dimensions);

            //return new Rastrigin(Number_Of_Dimensions);
            //return new Salomon(Number_Of_Dimensions);
        }
        
        static void Main(string[] args)
        {
            var problem = GetEvaluator();
            
            var runner = new ProblemPerformanceAssessor<double>(
                GetBuilder(problem.GetGlobalOptimum().GetDecisionSpace()), problem,
                p => p.AbsoluteDecisionVectorConvergence(Convergence_Tolerance));

            var results = runner.RunAssessment(
                Number_Of_Restarts,
                r => Console.Write("."),
                i =>
                {
                    Console.WriteLine(); 
                    Console.WriteLine($"Optimisation {i}/{Number_Of_Restarts} complete.");
                    Console.WriteLine(); 
                },
                timeOutEvaluations: (int)Math.Pow(Number_Of_Dimensions, 3.0) * 10);
            
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

        private static OptimiserBuilder GetBuilder(DecisionSpace space)
        {
            var builder = new NelderMeadBuilder(space);
            
            builder.AddHyperParameter(
                new VariableDiscrete(1, int.MaxValue,
                    name: HyperParameterNames.NumberOfDimensions),
                Number_Of_Dimensions);
            
            builder.AddHyperParameter(
                new VariableContinuous(
                    lowerBoundForGeneration: 0.0001,
                    upperBoundForGeneration: 1,
                    name: HyperParameterNames.SimplexStepCreationSize), 
                Simplex_Creation_Step_Size);

            return builder;
        }
    }
}