using System;
using System.Collections.Generic;
using Optimisation.Base.Management;
using Optimisation.Base.Runtime;
using Optimisation.Problems.SingleObjective;

namespace Optimisation.Problems.HyperparameterOptimisation
{
    public class ProblemPerformanceAssessor<T>
    {
        private readonly OptimiserBuilder builder;
        private readonly ProblemSingleObjective evaluator;
        private readonly Func<Population, bool> convergenceCheckers;
        
        public ProblemPerformanceAssessor(
            OptimiserBuilder builder,
            ProblemSingleObjective evaluator,
            Func<Population, bool> convergenceCheckers)
        {
            this.builder = builder;
            this.evaluator = evaluator;
            this.convergenceCheckers = convergenceCheckers;
        }

        public List<ProblemPerformanceSingleObjective> RunAssessment(
            int numberOfRestarts, 
            Action<Population> reporters, 
            Action<int> iterationReporter,
            int timeOutEvaluations = 0, 
            TimeSpan? timeOutDuration = null)
        { 
            var results = new List<ProblemPerformanceSingleObjective>();

            for (var i = 1; i <= numberOfRestarts; i++)
            {
                var timeStart = DateTime.Now;
                
                var optimiserRunner = new OptimiserRunnerBasic(builder, evaluator, convergenceCheckers, reporters);

                optimiserRunner.Run(
                    reportingFrequency: 1000,
                    timeOutEvaluations: timeOutEvaluations, 
                    timeOutDuration: timeOutDuration);

                results.Add(
                    new ProblemPerformanceSingleObjective(
                        builder.CreateOptimiser().ToString(),
                        builder.HyperParameters,
                        evaluator,
                        optimiserRunner));

                iterationReporter(i);
            }
            
            return results;
        }
    }
}