using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.NelderMead.Simplices
{
    /// <summary>
    /// Simplex Operator which performs Shrink operation (only along one dimension).
    /// </summary>
    public class Shrink : SimplexOperator
    {
        /// <inheritdoc />
        public Shrink(double coefficient) : base(coefficient)
        {
        }

        protected override bool CheckCoefficientAcceptable(double value)
        {
            if (value > 0 & value < 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Shrink operation moves the worst vertex a fraction along the line to the best vertex
        /// </summary>
        /// <param name="orderedVertices">All vertices</param>
        /// <returns>New vertex</returns>
        public override DecisionVector Operate(IEnumerable<DecisionVector> orderedVertices)
        {
            var worst = GetWorst(orderedVertices);
            var best = GetBest(orderedVertices);

            var newLocation = best.Vector.Select(
                (d,i) => (1 - Coefficient) * (double)d + 
                Coefficient * (double)worst.Vector[i]);

            return DecisionVector.CreateFromArray(
                best.GetDecisionSpace(),
                newLocation);
        }
    }
}
