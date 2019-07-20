using MathNet.Numerics;
using Optimisation.Base.Variables;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Problems.SingleObjective.Discrete
{
    public abstract class ProblemSingleObjectiveVehicleRouting : ProblemSingleObjective
    {
        protected double[,] locationDistanceMap;
        protected readonly IReadOnlyList<double[]> locations;

        public ProblemSingleObjectiveVehicleRouting(
            string name,
            IEnumerable<double[]> locations,
            DecisionVector globalOptimum)
            : base(name, globalOptimum,
                  VehicleRoutingProblemPropertyNames.VisitOrder, VehicleRoutingProblemPropertyNames.TotalDistance)
        {
            this.locations = locations.ToList();

            // Initialise distances
            CreateCityMap();
        }

        /// <summary>
        /// Pre-processor to work out distances between cities
        /// </summary>
        private void CreateCityMap()
        {
            locationDistanceMap = new double[locations.Count, locations.Count];
            for (var firstLoc = 0; firstLoc < locations.Count; firstLoc++)
            {
                for (var secondLoc = firstLoc + 1; secondLoc < locations.Count; secondLoc++)
                {
                    locationDistanceMap[firstLoc, secondLoc] = 
                        Distance.Euclidean(locations[firstLoc], locations[secondLoc]);
                    locationDistanceMap[secondLoc, firstLoc] = locationDistanceMap[firstLoc, secondLoc];
                }
            }
        }

        /// <summary>
        /// Gets total distance travelled.
        /// </summary>
        /// <param name="visitOrder">list of locations visited in order</param>
        /// <returns>total distance</returns>
        protected double CalculateTotalTravelDistance(IEnumerable<int> visitOrder)
        {
            double totalDistance = 0;
            for (var i = 1; i < visitOrder.Count(); i++)
            {
                totalDistance += locationDistanceMap[visitOrder.ElementAt(i - 1), visitOrder.ElementAt(i)];
            }
            return totalDistance;
        }
    }
}
