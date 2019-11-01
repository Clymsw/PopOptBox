using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Runtime
{
    /// <summary>
    /// The Reinsertion Agent receives evaluated individuals and outputs new ones.
    /// </summary>
    internal class ReinsertionAgent
    {
        /// <summary>
        /// Cancellation after certain conditions are met. Also used in <seealso cref="EvaluationAgent"/>.
        /// </summary>
        public readonly CancellationTokenSource CancellationSource;
        private readonly TimeOutManager timeOutManager;
        private readonly Func<Population, bool> convergenceCheckers;

        private readonly int numberOfNewIndividualsPerGeneration;
        
        /// <summary>
        /// The block that handles reinserting returned individuals 
        /// and then creating a new one.
        /// </summary>
        public TransformManyBlock<Individual, Individual> IndividualsForReinsertion { get; }

        /// <summary>
        /// The block that keeps new individuals until they are ready for evaluation.
        /// </summary>
        public BufferBlock<Individual> NewIndividuals { get; }

        /// <summary>
        /// The block that keeps progress reports until they are used.
        /// </summary>
        public BroadcastBlock<Population> Reports { get; }

        /// <summary>
        /// The number of reinsertions between updating the reporter.
        /// </summary>
        public readonly int ReportingFrequency;

        private readonly Optimiser optimiser;
        private readonly IModel model;

        /// <summary>
        /// The number of individuals that have been generated.
        /// </summary>
        public int NumberGenerated { get; private set; }

        /// <summary>
        /// The number of individuals that have been reinserted.
        /// </summary>
        public int NumberReinserted { get; private set; }

        /// <summary>
        /// <see langword="true"/> if all individuals will be stored.
        /// </summary>
        public bool SaveAll = false;

        /// <summary>
        /// All <see cref="Individual"/>s evaluated so far.
        /// </summary>
        public readonly List<Individual> AllEvaluated;

        /// <summary>
        /// Gets the current population in the <see cref="Optimiser"/>.
        /// </summary>
        /// <returns></returns>
        public Population GetCurrentPopulation()
        {
            return optimiser.Population;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="optimiser">The optimiser.</param>
        /// <param name="model">The model.</param>
        /// <param name="timeOutManager">The timeout manager.</param>
        /// <param name="convergenceCheckers">Checks for early termination.</param>
        /// <param name="reportingFrequency">Number of reinsertions between reports on progress.</param>
        /// <param name="numberOfNewIndividualsPerGeneration">Number of new individuals to generate whenever an individual is evaluated.</param>
        public ReinsertionAgent(
            Optimiser optimiser,
            IModel model,
            TimeOutManager timeOutManager,
            Func<Population, bool> convergenceCheckers,
            int reportingFrequency,
            int numberOfNewIndividualsPerGeneration)
        {
            CancellationSource = new CancellationTokenSource();
            this.timeOutManager = timeOutManager;
            this.convergenceCheckers = convergenceCheckers;
            NumberGenerated = 0;
            NumberReinserted = 0;
            AllEvaluated = new List<Individual>();

            ReportingFrequency = reportingFrequency;

            this.optimiser = optimiser;
            this.model = model;
            
            this.numberOfNewIndividualsPerGeneration = numberOfNewIndividualsPerGeneration;

            IndividualsForReinsertion = new TransformManyBlock<Individual, Individual>(
                (Func<Individual, IEnumerable<Individual>>)Process,
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

            Reports = new BroadcastBlock<Population>(i => i);

            //Set up link so that new individuals created 
            // are pushed to the output buffer
            IndividualsForReinsertion.LinkTo(NewIndividuals);
        }

        /// <summary>
        /// Does the reinsertion and manages new individual generation.
        /// </summary>
        /// <param name="returnedInd">Evaluated individual.</param>
        /// <returns>New individuals (1 or 0) ready for evaluation.</returns>
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
                AllEvaluated.Add(returnedInd);
            }

            // Check for completion
            var completed = timeOutManager.HasPerformedTooManyEvaluations() 
                || timeOutManager.HasRunOutOfTime() 
                || optimiser.Population.IsTargetSizeReached && convergenceCheckers(optimiser.Population);

            //Reporting
            if (timeOutManager.EvaluationsRun % ReportingFrequency == 0 || completed)
            {
                var pop = optimiser.Population.Clone();
                Reports.Post(pop);
            }

            //New individual (or nothing!)
            if (completed)
            {
                CancellationSource.Cancel();
                return new List<Individual>();
            }
            
            return CreateNewIndividuals(numberOfNewIndividualsPerGeneration);
        }

        /// <summary>
        /// Performs the creation of new individuals.
        /// </summary>
        /// <remarks>Public so it can be used to initialise the optimisation.</remarks>
        /// <param name="numberToCreate">The number of new individuals desired.</param>
        /// <returns>A list of new individuals ready for evaluation.</returns>
        public IReadOnlyList<Individual> CreateNewIndividuals(int numberToCreate)
        {
            var nextInds = optimiser.GetNextToEvaluate(numberToCreate);

            foreach (var ind in nextInds)
            {
                if (ind.DecisionVector.Count == 0)
                {
                    // Optimiser.GetNextDecisionVector() has generated a cancellation condition
                    CancellationSource.Cancel();
                }

                model.PrepareForEvaluation(ind);
                NumberGenerated++;
                ind.SetProperty(OptimiserPropertyNames.CreationIndex, NumberGenerated);
            }

            return nextInds;
        }
    }
}