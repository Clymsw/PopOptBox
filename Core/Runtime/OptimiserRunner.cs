using System.Collections.Generic;
using Optimisation.Base.Management;

namespace Optimisation.Base.Runtime
{
    public abstract class OptimiserRunner
    {
        public KeyValuePair<int, Individual> BestFound { get; protected set; }
        public List<KeyValuePair<int, Individual>> AllEvaluated { get; protected set; }
        public Population FinalPopulation { get; protected set; }

        public abstract void Run(bool storeAll = true, int timeOut = 0, int reportingFrequency = 100);
        public abstract void Cancel();
    }
}