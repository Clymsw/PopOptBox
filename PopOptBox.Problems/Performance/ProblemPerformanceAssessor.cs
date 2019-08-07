using System;
using System.Collections.Generic;
using PopOptBox.Base.Management;
using PopOptBox.Base.Runtime;
using PopOptBox.Problems.SingleObjective;

namespace PopOptBox.Problems.Performance
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
                        evaluator,
                        optimiserRunner));

                iterationReporter(i);
            }
            
            return results;
        }
    }
}