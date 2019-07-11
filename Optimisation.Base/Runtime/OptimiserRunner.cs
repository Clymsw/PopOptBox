using System;
using System.Collections.Generic;
using Optimisation.Base.Management;

namespace Optimisation.Base.Runtime
{
    public abstract class OptimiserRunner
    {
        public KeyValuePair<int, Individual> BestFound { get; protected set; }
        public List<KeyValuePair<int, Individual>> AllEvaluated { get; protected set; }
        public Population FinalPopulation { get; protected set; }

        public abstract void Run(bool storeAll = true, int reportingFrequency = 100, int timeOutEvaluations = 0, TimeSpan? timeOutDuration = null);
        public abstract void Cancel();
    }
}