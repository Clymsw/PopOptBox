using System;
using System.Collections.Generic;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Runtime
{
    /// <summary>
    /// The manager for running an optimisation. 
    /// Use <seealso cref="OptimiserRunnerBasic"/> or <seealso cref="OptimiserRunnerParallel"/>.
    /// </summary>
    public abstract class OptimiserRunner
    {
        /// <summary>
        /// The time the optimisation was started.
        /// </summary>
        public DateTime? StartTime { get; protected set; }

        /// <summary>
        /// The final result (single objective).
        /// </summary>
        public Individual? BestFound { get; protected set; }

        /// <summary>
        /// A stored list of all individuals which have been tried.
        /// </summary>
        public List<Individual>? AllEvaluated { get; protected set; }

        /// <summary>
        /// The final population the optimiser had when it terminated.
        /// </summary>
        public Population? FinalPopulation { get; protected set; }

        /// <summary>
        /// Runs the optimisation.
        /// </summary>
        /// <param name="storeAll"><see langword="true"/> to store all individuals evaluated (memory required).</param>
        /// <param name="reportingFrequency">The number of evaluations between reporting progress.</param>
        /// <param name="timeOutEvaluations">The maximum number of evaluations before terminating the optimisation.</param>
        /// <param name="timeOutDuration">The maximum time allowed before terminating the optimisation.</param>
        /// <param name="newIndividualsPerGeneration">The number of new <see cref="Individual"/>s to generate each time new individuals are generated from the <see cref="Population"/>.</param>
        public abstract void Run(bool storeAll = true, int reportingFrequency = 100, int timeOutEvaluations = 0, TimeSpan? timeOutDuration = null, int newIndividualsPerGeneration = 1);

        /// <summary>
        /// Stops the optimisation.
        /// </summary>
        public abstract void Cancel();
    }
}