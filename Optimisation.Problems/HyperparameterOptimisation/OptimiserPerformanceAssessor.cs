using System;
using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Conversion;
using Optimisation.Base.Management;
using Optimisation.Base.Runtime;

namespace Optimisation.Problems.HyperparameterOptimisation
{
    public class OptimiserPerformanceAssessor
    {
        private readonly OptimiserBuilder builder;
        private readonly IEvaluator evaluator;
        private readonly Func<Population, bool> convergenceCheckers;
        
        public OptimiserPerformanceAssessor(
            OptimiserBuilder builder, 
            IEvaluator evaluator,
            Func<Population, bool> convergenceCheckers)
        {
            this.builder = builder;
            this.evaluator = evaluator;
            this.convergenceCheckers = convergenceCheckers;
        }

        public OptimiserPerformance RunAssessment(int numberOfRestarts, 
            Action<Population> reporters, Action<int> iterationReporter,
            int timeOutEvaluations = 0, TimeSpan? timeOutDuration = null)
        { 
            var fitnessValues = new List<double>();
            var timeToComplete = new List<TimeSpan>();
            var evaluationsToComplete = new List<int>();
            var evaluationsToFindBest = new List<int>();

            for (var i = 1; i <= numberOfRestarts; i++)
            {
                var timeStart = DateTime.Now;
                
                var optimiserRunner = new OptimiserRunnerBasic(builder, evaluator, convergenceCheckers, reporters);
                optimiserRunner.Run(
                    reportingFrequency: 1000,
                    timeOutEvaluations: timeOutEvaluations, 
                    timeOutDuration: timeOutDuration);
                
                fitnessValues.Add(optimiserRunner.BestFound.Fitness);
                evaluationsToFindBest.Add(optimiserRunner.BestFound.GetProperty<int>(OptimiserPropertyNames.CreationIndex));
                evaluationsToComplete.Add(optimiserRunner.AllEvaluated.Count);
                timeToComplete.Add(
                    optimiserRunner.FinalPopulation
                        .Select(ind => ind.GetProperty<DateTime>(OptimiserPropertyNames.ReinsertionTime))
                        .OrderByDescending(t => t)
                        .First()
                    - timeStart);

                iterationReporter(i);
            }
            
            return new OptimiserPerformance(
                builder.CreateOptimiser().ToString(),
                builder.HyperParameters,
                evaluator.ToString(),
                fitnessValues, timeToComplete, evaluationsToComplete, evaluationsToFindBest);
        }
    }
}