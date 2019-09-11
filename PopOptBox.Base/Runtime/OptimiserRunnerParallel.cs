using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Runtime
{
    /// <summary>
    /// Implementation of <see cref="OptimiserRunner"/> which creates and evaluates <see cref="Individual"/>s asynchronously, using TPL.
    /// </summary>
    public sealed class OptimiserRunnerParallel : OptimiserRunner
    {
        /// <summary>
        /// The Agent that handles reinserting evaluated individuals and creating new ones
        /// </summary>
        private ReinsertionAgent reinsertionAgent;
        /// <summary>
        /// The Agent that handles evaluating individuals in parallel
        /// </summary>
        private EvaluationAgent evaluationAgent;

        /// <summary>
        /// The block that handles pushing reports to the reporting delegates provided
        /// </summary>
        private readonly ActionBlock<Population> reportingAgent;

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
        /// Constructor.
        /// </summary>
        /// <param name="builder">The builder for the optimisation elements.</param>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="convergenceCheckers">Checks for early completion.</param>
        /// <param name="reporters">The action that reports progress.</param>
        public OptimiserRunnerParallel(
            OptimiserBuilder builder,
            IEvaluator evaluator,
            Func<Population, bool> convergenceCheckers,
            Action<Population> reporters)
        {
            this.builder = builder;
            this.evaluator = evaluator;
            this.convergenceCheckers = convergenceCheckers;

            reportingAgent = new ActionBlock<Population>(
                reporters);

            NumberOfIndividualsToStart = builder.CreateOptimiser().Population.TargetSize / 4;
            if (NumberOfIndividualsToStart < 4)
                NumberOfIndividualsToStart = 4;
        }

        /// <summary>
        /// Cancels the optimisation (it will need to clear out the individuals being processed first).
        /// </summary>
        public override void Cancel()
        {
            reinsertionAgent.CancellationSource.Cancel();
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
            StartTime = DateTime.Now;

            // Calculate time outs automatically if not provided
            if (timeOutEvaluations == 0)
            {
                var numDims =
                    builder.CreateModel().GetNewDecisionVector().Count;
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
                Math.Max(NumberOfIndividualsToStart, newIndividualsPerGeneration));
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
            
            // Not really the best found unless the optimiser is elitist
            BestFound = FinalPopulation.Best();
        }

        /// <summary>
        /// Initialises all the buffers to be ready.
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