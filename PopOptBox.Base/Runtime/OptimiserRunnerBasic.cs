using System;
using System.Collections.Generic;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Runtime
{
    /// <summary>
    /// Implementation of <see cref="OptimiserRunner"/> which creates and evaluates <see cref="Individual"/>s synchronously.
    /// </summary>
    public sealed class OptimiserRunnerBasic : OptimiserRunner
    {
        private readonly OptimiserBuilder builder;
        private readonly IEvaluator evaluator;
        private readonly Func<Population, bool> convergenceCheckers;
        private readonly Action<Population> reporters;

        private TimeOutManager timeOutManager;
        private volatile bool cancelDemanded;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The builder for the optimisation elements.</param>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="convergenceCheckers">Checks for early completion.</param>
        /// <param name="reporters">The action that reports progress.</param>
        public OptimiserRunnerBasic(
            OptimiserBuilder builder,
            IEvaluator evaluator,
            Func<Population, bool> convergenceCheckers,
            Action<Population> reporters)
        {
            this.builder = builder;
            this.evaluator = evaluator;
            this.convergenceCheckers = convergenceCheckers;
            this.reporters = reporters;
        }

        /// <summary>
        /// Stops the optimisation.
        /// </summary>
        public override void Cancel()
        {
            cancelDemanded = true;
        }

        /// <summary>
        /// Runs the optimisation.
        /// </summary>
        /// <param name="storeAll"><see langword="true"/> to store all individuals evaluated (memory required).</param>
        /// <param name="reportingFrequency">The number of evaluations between reporting progress.</param>
        /// <param name="timeOutEvaluations">The maximum number of evaluations before terminating the optimisation.</param>
        /// <param name="timeOutDuration">The maximum time allowed before terminating the optimisation.</param>
        /// <param name="newIndividualsPerGeneration">The number of new <see cref="Individual"/>s to generate each time new individuals are generated from the <see cref="Population"/>.</param>
        public override void Run(
            bool storeAll = true, 
            int reportingFrequency = 100, 
            int timeOutEvaluations = 0, 
            TimeSpan? timeOutDuration = null,
            int newIndividualsPerGeneration = 1)
        {
            // Initialise
            StartTime = DateTime.Now;
            var optimiser = builder.CreateOptimiser();
            var model = builder.CreateModel();
            cancelDemanded = false;

            //Setup
            var nextInds = optimiser.GetNextToEvaluate(newIndividualsPerGeneration);

            if (timeOutEvaluations == 0)
            {
                var numDims = nextInds[0].DecisionVector.Count;
                timeOutEvaluations = Math.Min(numDims * 20000, 2000000);
            }

            var timeOutDurationNotNull = TimeSpan.MaxValue;
            if (timeOutDuration != null)
            {
                timeOutDurationNotNull = timeOutDuration.Value;
            }

            timeOutManager = new TimeOutManager(timeOutEvaluations, timeOutDurationNotNull);

            AllEvaluated = new List<Individual>();
            FinalPopulation = null;
            BestFound = null;

            //Go!
            while (nextInds.Count > 0 && nextInds[0].DecisionVector.Count > 0)
            {
                foreach (var nextInd in nextInds)
                {
                    nextInd.SetProperty(
                        OptimiserPropertyNames.CreationIndex,
                        timeOutManager.EvaluationsRun);

                    // Evaluate
                    model.PrepareForEvaluation(nextInd);
                    evaluator.Evaluate(nextInd);
                }

                // Reinsert
                optimiser.ReInsert(nextInds);

                foreach (var nextInd in nextInds)
                {
                    nextInd.SetProperty(
                        OptimiserPropertyNames.ReinsertionIndex,
                        timeOutManager.EvaluationsRun);
                    timeOutManager.IncrementEvaluationsRun();
                }

                // Store
                if (storeAll)
                {
                    AllEvaluated.AddRange(nextInds);
                }

                // Update best
                var bestInd = optimiser.Population.Best();
                if (BestFound == null || (bestInd != null && bestInd.Fitness < BestFound.Fitness))
                    BestFound = bestInd;

                // Create individuals for next loop
                nextInds = optimiser.GetNextToEvaluate(newIndividualsPerGeneration);
                
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
                    reporters(optimiser.Population);
                }
            }

            reporters(optimiser.Population);

            //Finish off
            FinalPopulation = optimiser.Population;
        }
    }
}