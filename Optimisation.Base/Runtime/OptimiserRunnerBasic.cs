using System;
using System.Collections.Generic;
using Optimisation.Base.Management;
using Optimisation.Base.Conversion;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Runtime
{
    public sealed class OptimiserRunnerBasic : OptimiserRunner
    {
        private readonly OptimiserBuilder builder;
        private readonly IEvaluator evaluator;
        private readonly Func<Population, bool> convergenceCheckers;
        private readonly Action<(int, Population)> reporters;

        private TimeOutManager timeOutManager;
        private volatile bool cancelDemanded;

        public OptimiserRunnerBasic(
            OptimiserBuilder builder,
            IEvaluator evaluator,
            Func<Population, bool> convergenceCheckers,
            Action<(int, Population)> reporters)
        {
            this.builder = builder;
            this.evaluator = evaluator;
            this.convergenceCheckers = convergenceCheckers;
            this.reporters = reporters;
        }

        public override void Cancel()
        {
            cancelDemanded = true;
        }

        public override void Run(
            bool storeAll = true, 
            int reportingFrequency = 100, 
            int timeOutEvaluations = 0, 
            TimeSpan? timeOutDuration = null)
        {
            // Initialise
            StartTime = DateTime.Now;
            var optimiser = builder.CreateOptimiser();
            var model = builder.CreateModel();
            cancelDemanded = false;

            //Setup
            var nextInds = optimiser.GetNextToEvaluate(1);
            var nextInd = nextInds[0];

            if (timeOutEvaluations == 0)
            {
                var numDims = nextInd.DecisionVector.Vector.Count;
                timeOutEvaluations = Math.Min(numDims * 20000, 2000000);
            }

            var timeOutDurationNotNull = TimeSpan.MaxValue;
            if (timeOutDuration != null)
            {
                timeOutDurationNotNull = timeOutDuration.Value;
            }

            timeOutManager = new TimeOutManager(timeOutEvaluations, timeOutDurationNotNull);

            AllEvaluated = new List<(int, Individual)>();
            FinalPopulation = null;
            BestFound = (0, nextInd);

            //Go!
            while (nextInd.DecisionVector.Vector.Count > 0)
            {
                nextInd.SetProperty(
                    OptimiserPropertyNames.CreationIndex,
                    timeOutManager.EvaluationsRun);

                // Evaluate
                model.PrepareForEvaluation(nextInd);
                evaluator.Evaluate(nextInd);

                // Reinsert
                var returnInds = new List<Individual>
                {
                    nextInd
                };
                optimiser.ReInsert(returnInds);

                nextInd.SetProperty(
                    OptimiserPropertyNames.ReinsertionIndex,
                    timeOutManager.EvaluationsRun);

                // Store
                if (storeAll)
                {
                    AllEvaluated.Add((timeOutManager.EvaluationsRun, nextInd));
                }

                // Update best
                var bestInd = optimiser.Population.Best();
                if (bestInd != null && bestInd.Fitness < BestFound.Item2.Fitness)
                    BestFound = (timeOutManager.EvaluationsRun, bestInd);

                // Create individuals for next loop
                timeOutManager.IncrementEvaluationsRun();

                try
                {
                    nextInds = optimiser.GetNextToEvaluate(1);
                    nextInd = nextInds[0];
                }
                catch (Exception e)
                {
                    nextInd = new Individual(DecisionVector.CreateForEmpty());
                    nextInd.SetProperty(OptimiserPropertyNames.GenerationError, e);
                }

                // Check for completion
                if (timeOutManager.HasPerformedTooManyEvaluations() 
                    || timeOutManager.HasRunOutOfTime() 
                    || cancelDemanded)
                {
                    //Bored...
                    break;
                }
                if (optimiser.Population.IsTargetSizeReached)
                {
                    if (convergenceCheckers(optimiser.Population))
                        break;
                }
                if (timeOutManager.EvaluationsRun % reportingFrequency == 0)
                {
                    reporters((timeOutManager.EvaluationsRun, optimiser.Population));
                }
            }

            reporters((timeOutManager.EvaluationsRun, optimiser.Population));

            //Finish off
            FinalPopulation = optimiser.Population;
        }
    }
}