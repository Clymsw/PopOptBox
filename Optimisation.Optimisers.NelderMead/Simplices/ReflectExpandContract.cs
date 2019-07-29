using System.Linq;
using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.NelderMead.Simplices
{
    /// <summary>
    /// Simplex Operator which (depending on coefficient value) performs
    /// Reflect, Expand, Contract In or Contract Out operations.
    /// </summary>
    public class ReflectExpandContract : SimplexOperator
    {
        /// <inheritdoc />
        public ReflectExpandContract(double coefficient) : base(coefficient)
        {
        }

        protected override bool CheckCoefficientAcceptable(double value)
        {
            return value > -1;
        }

        /// <summary>
        /// Reflect, Contract and Expand moves the worst vertex along a line perpendicular
        /// to the plane formed by the other vertices.
        /// </summary>
        /// <param name="simplex">The simplex</param>
        /// <returns>New vertex</returns>
        public override DecisionVector Operate(Simplex simplex)
        {
            var mean = GetMean(simplex);
            var worst = simplex.Worst().DecisionVector;

            var newLocation = mean.Vector.Select(
                (d, i) => (1 + Coefficient) * (double)d -
                Coefficient * (double)worst.Vector[i]);

            return DecisionVector.CreateFromArray(
                worst.GetDecisionSpace(),
                newLocation);
        }
        
        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (!(obj is ReflectExpandContract other))
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
