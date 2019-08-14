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
            int reportingFrequency,
            Action<Population> reporters, 
            Action<int> iterationReporter,
            int timeOutEvaluations = 0, 
            TimeSpan? timeOutDuration = null,
            int numberOfNewIndividualsPerGeneration = 1)
        { 
            var results = new List<ProblemPerformanceSingleObjective>();

            for (var i = 1; i <= numberOfRestarts; i++)
            {
                var optimiserRunner = new OptimiserRunnerBasic(builder, evaluator, convergenceCheckers, reporters);

                optimiserRunner.Run(
                    reportingFrequency: reportingFrequency,
                    timeOutEvaluations: timeOutEvaluations, 
                    timeOutDuration: timeOutDuration,
                    newIndividualsPerGeneration: numberOfNewIndividualsPerGeneration);

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