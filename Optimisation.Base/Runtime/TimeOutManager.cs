using System;

namespace Optimisation.Base.Runtime
{
    /// <summary>
    /// The TimeOutManager collects all different ways in which an optimisation can be terminated early 
    /// (except by direct request).
    /// </summary>
    internal class TimeOutManager
    {
        private readonly int timeOutEvaluations;
        private readonly TimeSpan timeOutDuration;

        /// <summary>
        /// Number of evaluations run so far.
        /// <seealso cref="IncrementEvaluationsRun"/>
        /// </summary>
        public int EvaluationsRun { get; private set; }
        private readonly DateTime timeStarted;

        /// <summary>
        /// Constructs the manager with defined timeout rules.
        /// </summary>
        /// <param name="timeOutEvaluations">Maximum number of evaluations to be run.</param>
        /// <param name="timeOutDuration">Maximum time to run.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if any timeout value is invalid.</exception>
        public TimeOutManager(int timeOutEvaluations, TimeSpan timeOutDuration)
        {
            if (timeOutEvaluations < 2)
                throw new ArgumentOutOfRangeException(nameof(timeOutEvaluations), "Timeout after too few evaluations.");
            if (timeOutDuration < TimeSpan.FromSeconds(30))
                throw new ArgumentOutOfRangeException(nameof(timeOutDuration), "Timeout duration is too short.");

            this.timeOutEvaluations = timeOutEvaluations;
            this.timeOutDuration = timeOutDuration;
            EvaluationsRun = 0;
            timeStarted = DateTime.Now;
        }

        /// <summary>
        /// Gets whether the optimisation has been running too long (in time).
        /// </summary>
        /// <returns><see langword="true"/> if the optimisation should be terminated.</returns>
        public bool HasRunOutOfTime()
        {
            return (DateTime.Now - timeStarted) > timeOutDuration;
        }

        /// <summary>
        /// Gets whether the optimisation has been running too long (in evaluations).
        /// </summary>
        /// <returns><see langword="true"/> if the optimisation should be terminated.</returns>
        public bool HasPerformedTooManyEvaluations()
        {
            return EvaluationsRun >= timeOutEvaluations;
        }

        /// <summary>
        /// Increments number of evaluations run by 1.
        /// </summary>
        public void IncrementEvaluationsRun()
        {
            EvaluationsRun++;
        }
    }
}