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
            return value > 0 & value < 1;
        }

        /// <summary>
        /// Shrink operation moves the worst vertex a fraction along the line to the best vertex
        /// </summary>
        /// <param name="simplex">The simplex.</param>
        /// <returns>New vertex</returns>
        public override DecisionVector Operate(Simplex simplex)
        {
            var worst = simplex.Worst().DecisionVector;
            var best = simplex.Best().DecisionVector;

            var newLocation = best.Vector.Select(
                (d,i) => (1 - Coefficient) * (double)d + 
                Coefficient * (double)worst.Vector[i]);

            return DecisionVector.CreateFromArray(
                best.GetDecisionSpace(),
                newLocation);
        }
        
        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (!(obj is Shrink other))
                return false;

            return Coefficient == other.Coefficient;
        }

        public override int GetHashCode()
        {
            return new
            {
                Coefficient
            }.GetHashCode();
        }

        #endregion
    }
}
