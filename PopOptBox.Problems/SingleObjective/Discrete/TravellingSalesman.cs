using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.SingleObjective.Discrete
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
                        numDims + 1, 0, setup.Locations.Count - 1),
                    setup.OptimumRoute));
        }

        public override IEnumerable<double> Evaluate(DecisionVector definition)
        {
            if (definition.Vector.Count != globalOptimum.Vector.Count)
                throw new System.ArgumentOutOfRangeException(nameof(definition),
                    $"Route should be a complete tour of {globalOptimum.Vector.Count} stops.");

            return new[] { CalculateTotalTravelDistance(definition.Vector.Select(l => (int)l)) };
        }
    }
}
