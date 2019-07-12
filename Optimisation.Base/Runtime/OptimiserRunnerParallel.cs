using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Optimisation.Base.Conversion;
using Optimisation.Base.Management;

namespace Optimisation.Base.Runtime
{
    /// <summary>
    /// Manages running an optimisation with parallel evaluations
    /// </summary>
    public sealed class OptimiserRunnerParallel : OptimiserRunner
    {
        /// <summary>
        /// The Agent that handes reinserting evaluated individuals and creating new ones
        /// </summary>
        private ReinsertionAgent reinsertionAgent;
        /// <summary>
        /// The Agent that handles evaluating individuals in parallel
        /// </summary>
        private EvaluationAgent evaluationAgent;

        /// <summary>
        /// The block that handles pushing reports to the reporting delegates provided
        /// </summary>
        private readonly ActionBlock<(int, Population)> reportingAgent;

        private readonly OptimiserBuilder builder;
        private readonly IEvaluator evaluator;
        private readonly Func<Population, bool> convergenceCheckers;

        /// <summary>
        /// At the beginning the buffer for evaluation is primed with
        /// a certain number of individuals. Define that number here.
        /// The number of simultaneous evaluations (in the EvaluationAgent)
        /// is unbounded and therefore handled by the thread pool automatically.
        /// </summary>
        public readonly int NumberOfIndividualsToStart;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="builder">optimiser builder</param>
        /// <param name="evaluator">evaluator</param>
        /// <param name="convergenceCheckers">checks for early completion</param>
        /// <param name="reporters">action that reports progress.
        /// The integer represents the number of processed individuals (population generation)</param>
        public OptimiserRunnerParallel(
            OptimiserBuilder builder,
            IEvaluator evaluator,
            Func<Population, bool> convergenceCheckers,
            Action<(int, Population)> reporters)
        {
            this.builder = builder;
            this.evaluator = evaluator;
            this.convergenceCheckers = convergenceCheckers;

            reportingAgent = new ActionBlock<(int, Population)>(
                reporters);

            NumberOfIndividualsToStart = builder.CreateOptimiser().Population.TargetSize / 4;
            if (NumberOfIndividualsToStart < 4)
                NumberOfIndividualsToStart = 4;
        }

        public override void Cancel()
        {
            reinsertionAgent.CancellationSource.Cancel();
        }

        /// <summary>
        /// Do the optimiser by calling this!
        /// </summary>
        /// <param name="storeAll">Whether to store every individual evaluated or not</param>
        /// <param name="reportingFrequency">The number of re-insertions between reports on the current population</param>
        /// <param name="timeOutEvaluations">The maximum number of re-insertions before optimisation completion</param>
        /// <param name="timeOutDuration">The maximum amount of time before optimisation completion</param>
        public override void Run(
            bool storeAll = true,
            int reportingFrequency = 100,
            int timeOutEvaluations = 0,
            TimeSpan? timeOutDuration = null)
        {
            // Calculate time outs automatically if not provided
            if (timeOutEvaluations == 0)
            {
                var numDims =
                    builder.CreateModel().GetNewIndividual().DecisionVector.Vector.Count;
                timeOutEvaluations = Math.Min(numDims * 20000, 2000000);
            }

            var timeOutDurationNotNull = TimeSpan.MaxValue;
            if (timeOutDuration != null)
            {
                timeOutDurationNotNull = timeOutDuration.Value;
            }

            var timeOutManager = new TimeOutManager(timeOutEvaluations, timeOutDurationNotNull);

            SetUpAgents(timeOutManager, reportingFrequency);

            reinsertionAgent.SaveAll = storeAll;

            // Get started
            var pumpPrimingInds = reinsertionAgent.CreateNewIndividuals(
                NumberOfIndividualsToStart);
            foreach (var ind in pumpPrimingInds)
            {
                reinsertionAgent.NewIndividuals.Post(ind);
            }

            // Wait for completion
            try
            {
                Task.WaitAll(
                    reinsertionAgent.IndividualsForReinsertion.Completion,
                    reinsertionAgent.NewIndividuals.Completion,
                    evaluationAgent.EvaluatedIndividuals.Completion,
                    evaluationAgent.IndividualsForEvaluation.Completion);
            }
            catch (AggregateException e) when (e.InnerExceptions.All(ie => ie is TaskCanceledException))
            {
                // There is no way to wait for cancellation,
                // so we have to wait for completion and then
                // ignore the cancellation errors
            }

            FinalPopulation = reinsertionAgent.GetCurrentPopulation();
            AllEvaluated = reinsertionAgent.AllEvaluated;

            if (FinalPopulation.Count <= 0) 
                return;
            
            var best = FinalPopulation.Best();

            BestFound = new KeyValuePair<int, Individual>(
                best.GetProperty<int>(OptimiserPropertyNames.CreationIndex),
                best);
        }

        /// <summary>
        /// Initialises all the buffers to be ready
        /// </summary>
        /// <param name="timeOutManager">The <see cref="TimeOutManager"/>.</param>
        /// <param name="reportingFrequency">The number of reinsertions between reports on the current population</param>
        private void SetUpAgents(
            TimeOutManager timeOutManager,
            int reportingFrequency)
        {
            reinsertionAgent = new ReinsertionAgent(
                builder.CreateOptimiser(),
                builder.CreateModel(),
                timeOutManager, convergenceCheckers, reportingFrequency);

            evaluationAgent = new EvaluationAgent(
                evaluator, reinsertionAgent.CancellationSource.Token);

            //Create link so that newly created individuals from the ReinsertionAgent
            // are pushed to the EvaluationAgent
            reinsertionAgent.NewIndividuals.LinkTo(
                evaluationAgent.IndividualsForEvaluation);

            //Create link so that evaluated individuals from the EvaluationAgent
            // are pushed to the ReinsertionAgent
            evaluationAgent.EvaluatedIndividuals.LinkTo(
                reinsertionAgent.IndividualsForReinsertion);

            //Create link so that reports from the ReinsertionAgent
            // are pushed to the reporting delegates
            reinsertionAgent.Reports.LinkTo(reportingAgent);
        }
    }
}