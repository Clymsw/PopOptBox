using System.Collections.Generic;

namespace Optimisation.Problems.SingleObjective.Discrete
{
    internal class TravellingSalesmanSetup
    {
        public TravellingSalesmanSetup(string filePath)
        {

        }

        public readonly string ShortName;
        public readonly string LongName;
        public readonly List<double[]> Locations;
        public readonly int[] OptimumRoute;


    }
}
