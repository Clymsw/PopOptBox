using System;
using System.Linq;
using Optimisation.Base.Helpers;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using Optimisation.Problems.Continuous;
using Optimisation.Problems.Continuous.SingleObjective;
using Optimisation.Problems.HyperparameterOptimisation;

namespace Optimisation.HyperParameterTuning
{
    class Program
    {
        private const int Number_Of_Dimensions = 20;
        private const double Simplex_Creation_Step_Size = 0.5;
        private const double Convergence_Tolerance = 0.00001;
        private const int Number_Of_Restarts = 100;
        private const double Fitness_Tolerance = 0.01;
        
        private static ProblemEvaluatorSingleObjective GetEvaluator()
        {
            return new Ellipsoidal(Number_Of_Dimensions);
//            return new Schwefel(Number_Of_Dimensions);
//            return new Rosenbrock(Number_Of_Dimensions);
        }
        
        static void Main(string[] args)
        {
            var problem = GetEvaluator();
            
            var runner = new OptimiserPerformanceAssessor(
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
                timeOutEvaluations: 10000);
            
            Console.WriteLine();
            var globalOptimumLocation = problem.GetGlobalOptimum().Vector.Select(d => (double) d).ToArray();
            var globalOptimumFitness = problem.Evaluate(globalOptimumLocation).ElementAt(0);
            Console.WriteLine($"Global optimum fitness: {globalOptimumFitness.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Best fitness found: {results.BestFitness.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}");
            var pctNearOptimum = results.ProportionOfFitnessValuesWithin(Fitness_Tolerance) * 100;
            Console.WriteLine($"Proportion of results below {Fitness_Tolerance - globalOptimumFitness}: {pctNearOptimum.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}%");
            Console.WriteLine($"Mean number of evaluations required to find best fitness: {results.MeanEvaluationsToFindBest.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Mean fitness found: {results.MeanFitness.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Mean number of evaluations required to converge: {results.MeanEvaluationsToConverge.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Mean time required to converge: {results.MeanTimeToConverge.TotalSeconds.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)} seconds");

            Console.ReadKey();
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