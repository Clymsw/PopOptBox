using Optimisation.Base.Management;
using Optimisation.Base.Variables;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Optimisers.NelderMead.Simplex
{
    /// <summary>
    /// The Simplex is a collection of Vertices (points in space: Individuals)
    /// which always has the length of D+1, where D is the number of dimensions.
    /// </summary>
    public class Simplex : Population
    {

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialSimplex"></param>
        public Simplex(IEnumerable<Individual> initialSimplex) :
            base(initialSimplex.First().DecisionVector.Vector.Count + 1, initialSimplex)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfDimensions"></param>
        public Simplex(int numberOfDimensions) : base(numberOfDimensions + 1)
        {
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialVertex"></param>
        /// <param name="stepSize"></param>
        /// <returns></returns>
        public static List<Individual> CreateInitialVertices(
            Individual initialVertex, double stepSize)
        {
            var simplex = new List<Individual>
            {
                initialVertex
            };

            var startDv = initialVertex.DecisionVector.Vector.Select(d => (double)d).ToArray();

            for (int i = 2; i <= startDv.Length + 1; i++)
            {
                // Create D+1 total vertices.
                var newDv = new double[startDv.Length];
                startDv.CopyTo(newDv, 0);

                // Each vertex has one of its dimensions offset by an amount equal to stepsize.
                newDv[i - 2] += stepSize;

                simplex.Add(new Individual(
                    DecisionVector.CreateFromArray(
                        initialVertex.DecisionVector.GetDecisionSpace(),
                        newDv)));
            }
            return simplex;
        }
    }
}
