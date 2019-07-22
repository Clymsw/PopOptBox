using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Problems.SingleObjective.Discrete.Test
{
    public class ProblemSingleObjectiveVehicleRoutingTests
    {
        [Theory]
        [InlineData(new[] { 0.0, 3, 4, 0, 5, 0 }, new[] { 0.0, 0 }, new[] { 0.0, 3 }, new[] { 4.0, 0 })]
        public void ThreeCitiesInATriangle_CorrectlyCalculatesDistances(double[] distances, params double[][] cityLocations)
        {
            var twoCityDs = DecisionSpace.CreateForUniformIntArray(2, 0, distances.Length - 1);

            var vrp = new ProblemSingleObjectiveVehicleRoutingMock("Test Triangle", cityLocations,
                DecisionVector.CreateFromArray(twoCityDs, new[] { 0, 1 }));

            int k = 0;
            for (int i = 0; i < cityLocations[0].Length - 1; i++)
            {
                for (int j = i; j < cityLocations[0].Length - 1; j++)
                {
                    Assert.Equal(distances[k],
                        vrp.Evaluate(DecisionVector.CreateFromArray(twoCityDs, new[] { i, j })).ElementAt(0));
                    k++;
                }
            }
        }
    }

    internal class ProblemSingleObjectiveVehicleRoutingMock : ProblemSingleObjectiveVehicleRouting
    {
        public ProblemSingleObjectiveVehicleRoutingMock(
            string name,
            IEnumerable<double[]> locations,
            DecisionVector globalOptimum) : base(name, locations, globalOptimum)
        {
        }

        public override IEnumerable<double> Evaluate(DecisionVector definition)
        {
            return new[] { CalculateTotalTravelDistance(definition.Vector.Select(x => (int)x)) };
        }
    }
}
