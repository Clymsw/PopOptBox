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
        }

        public static TravellingSalesman CreateFromFile(string filePath)
        {
            var setup = new TravellingSalesmanSetup(filePath);
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
