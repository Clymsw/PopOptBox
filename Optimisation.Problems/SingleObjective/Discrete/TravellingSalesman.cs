using System.Collections.Generic;
using System.IO;
using System.Linq;
using Optimisation.Base.Variables;

namespace Optimisation.Problems.SingleObjective.Discrete
{
    public class TravellingSalesman : ProblemSingleObjectiveVehicleRouting
    {
        public TravellingSalesman(string name,
            IEnumerable<double[]> locations, 
            DecisionVector globalOptimum) 
            : base(name, locations, globalOptimum)
        {
            if (globalOptimum.Vector.Count != locations.Count() + 1)
                throw new System.ArgumentOutOfRangeException(
                    nameof(globalOptimum), 
                    $"The optimum route should be a complete tour of {locations.Count() + 1} stops.");
        }

        public static TravellingSalesman CreateFromFile(string problemFilePath)
        {
            var setup = new TravellingSalesmanSetup(problemFilePath);
            int numDims = setup.Locations.Count;
            return new TravellingSalesman(
                setup.LongName,
                setup.Locations,
                DecisionVector.CreateFromArray(
                    DecisionSpace.CreateForUniformIntArray(
                        numDims, 0, setup.Locations.First().Count() - 1),
                    setup.OptimumRoute));
        }

        public override IEnumerable<double> Evaluate(DecisionVector definition)
        {
            return new[] { CalculateTotalTravelDistance(definition.Vector.Select(l => (int)l)) };
        }

    }
}
