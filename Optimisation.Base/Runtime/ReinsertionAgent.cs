using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Optimisation.Base.Conversion;
using Optimisation.Base.Management;

namespace Optimisation.Base.Runtime
{
    /// <summary>
    /// The Reinsertion Agent receives evaluated individuals and outputs new ones
    /// </summary>
    internal class ReinsertionAgent
    {
        /// <summary>
        /// Cancellation after certain conditions are met. Also used in EvaluationAgent
        /// </summary>
        public readonly CancellationTokenSource CancellationSource;
        private readonly TimeOutManager timeOutManager;
        private readonly Func<Population, bool> convergenceCheckers;

        /// <summary>
        /// The block that handles reinserting returned individuals 
        /// and then creating a new one
        /// </summary>
        public TransformManyBlock<Individual, Individual> IndividualsForReinsertion { get; }

        /// <summary>
        /// The block that keeps new individuals until they are ready for evaluation
        /// </summary>
        public BufferBlock<Individual> NewIndividuals { get; }

        /// <summary>
        /// The block that keeps progress reports until they are used
        /// </summary>
        public BroadcastBlock<(int, Population)> Reports { get; }

        public readonly int ReportingFrequency;

        private readonly Optimiser optimiser;
        private readonly IModel model;

        public int NumberGenerated { get; private set; }
        public int NumberReinserted { get; private set; }

        public bool SaveAll = false;
        public readonly List<KeyValuePair<int, Individual>> AllEvaluated;

        public Population GetCurrentPopulation()
        {
            return optimiser.Population;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="optimiser">The optimiser</param>
        /// <param name="model">The model</param>
        /// <param name="timeOut">Max number of reinsertions until cancellation</param>
        /// <param name="convergenceCheckers">Checks for early termination</param>
        /// <param name="reportingFrequency">Number of reinsertions between reports on current population</param>
        public ReinsertionAgent(
            Optimiser optimiser,
            IModel model,
            TimeOutManager timeOutManager,
            Func<Population, bool> convergenceCheckers,
            int reportingFrequency)
        {
            CancellationSource = new CancellationTokenSource();
            this.timeOutManager = timeOutManager;
            this.convergenceCheckers = convergenceCheckers;
            NumberGenerated = 0;
            NumberReinserted = 0;
            AllEvaluated = new List<KeyValuePair<int, Individual>>();

            ReportingFrequency = reportingFrequency;

            this.optimiser = optimiser;
            this.model = model;

            IndividualsForReinsertion = new TransformManyBlock<Individual, Individual>(
                Process,
                new ExecutionDataflowBlockOptions
                {
                    CancellationToken = CancellationSource.Token,
                    MaxDegreeOfParallelism = 1
                });

            NewIndividuals = new BufferBlock<Individual>(
                new DataflowBlockOptions
                {
                    CancellationToken = CancellationSource.Token
                });

            Reports = new BroadcastBlock<(int, Population)>(i => i);

            //Set up link so that new individuals created 
            // are pushed to the output buffer
            IndividualsForReinsertion.LinkTo(NewIndividuals);
        }

        /// <summary>
        /// Does the reinsertion and manages new individual generation
        /// </summary>
        /// <param name="returnedInd">evaluated individual</param>
        /// <returns>new individual ready for evaluation</returns>
        private IReadOnlyList<Individual> Process(Individual returnedInd)
        {
            timeOutManager.IncrementEvaluationsRun();
            var returnInds = new List<Individual>
            {
                returnedInd
            };

            var numReInserted = optimiser.ReInsert(returnInds);
            if (numReInserted > 0)
            {
                NumberReinserted += numReInserted;
                returnedInd.SetProperty(
                    OptimiserPropertyNames.ReinsertionIndex,
                    NumberReinserted);
            }

            if (SaveAll)
            {
                AllEvaluated.Add(new KeyValuePair<int, Individual>(
                    timeOutManager.EvaluationsRun, returnedInd));
            }

            // Check for completion
            var completed = timeOutManager.HasPerformedTooManyEvaluations() 
                || timeOutManager.HasRunOutOfTime() 
                || optimiser.Population.IsTargetSizeReached && convergenceCheckers(optimiser.Population);

            //Reporting
            if (timeOutManager.EvaluationsRun % ReportingFrequency == 0 || completed)
            {
                var pop = optimiser.Population.Clone();
                Reports.Post((timeOutManager.EvaluationsRun, pop));
            }

            //New individual (or nothing!)
            if (completed)
            {
                CancellationSource.Cancel();
                return new List<Individual>();
            }
            else
            {
                return CreateNewIndividuals(1);
            }
        }

        /// <summary>
        /// Does the creation of new individuals.
        /// Public so it can be used to initialise the optimisation
        /// </summary>
        /// <param name="numberToCreate">number of new individuals desired</param>
        /// <returns>list of new individuals ready for evaluation</returns>
        public IReadOnlyList<Individual> CreateNewIndividuals(int numberToCreate)
        {
            var nextInds = optimiser.GetNextToEvaluate(numberToCreate);
            foreach (var ind in nextInds)
            {
                model.PrepareForEvaluation(ind);
                NumberGenerated++;
                ind.SetProperty(
                    OptimiserPropertyNames.CreationIndex,
                    NumberGenerated);
            }
            return nextInds;
        }
    }
}