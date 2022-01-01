using System;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.StructuredSearch.Simplices
{
    /// <summary>
    /// A simplex operator creates a new vertex based on an existing simplex,
    /// using deterministic logic.
    /// </summary>
    public abstract class SimplexOperator : ISimplexOperator
    {
        #region Fields

        private double? coefficient;
        private const double Default_Coefficient = -9999;
        /// <summary>
        /// Variable affecting the behaviour of <see cref="Operate(IEnumerable{DecisionVector})"/>.
        /// </summary>
        public double Coefficient
        {
            get => coefficient.GetValueOrDefault(Default_Coefficient);
            set
            {
                if (CheckCoefficientAcceptable(value))
                    coefficient = value;
            }
        }

        protected abstract bool CheckCoefficientAcceptable(double value);

        #endregion

        #region Constructor

        /// <summary>
        /// Construct the simplex operator.
        /// </summary>
        /// <param name="coefficient">Value to set for the <see cref="Coefficient"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coefficient value fails legality check.</exception>
        protected SimplexOperator(double coefficient)
        {
            Coefficient = coefficient;
            // This slightly contorted logic enables the user to change the coefficient value during an optimisation
            // without having to create a new SimplexOperator.
            if (Coefficient == Default_Coefficient)
                throw new ArgumentOutOfRangeException(nameof(coefficient),"Coefficient value is not acceptable.");
        }

        #endregion

        #region Implement Interface

        /// <summary>
        /// Returns a new vertex location based on an existing vertex.
        /// </summary>
        /// <param name="simplex">The simplex.</param>
        /// <returns>A <see cref="DecisionVector"/> for the new vertex.</returns>
        public abstract DecisionVector Operate(Simplex simplex);

        #endregion

        protected static DecisionVector GetMean(Simplex simplex)
        {
            var numDims = simplex.Count;

            // Average of all vertex locations except the worst.
            var allVertexVectorsExceptWorst = simplex.GetMemberDecisionVectors()
                .Take(numDims - 1)
                .Select(v => v.Select(d => (double)d))
                .Select(v => CreateVector.DenseOfArray(v.ToArray()));

            var centroid = allVertexVectorsExceptWorst.Aggregate((a, b) => a + b) / (numDims - 1);

            return DecisionVector.CreateFromArray(
                simplex.Best().DecisionVector.GetDecisionSpace(),
                centroid.AsArray());
        }

        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (obj is not ReflectExpandContract other)
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
