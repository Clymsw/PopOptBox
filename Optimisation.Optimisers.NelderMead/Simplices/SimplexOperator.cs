using System;
using System.Linq;
using System.Collections.Generic;
using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.NelderMead.Simplices
{
    /// <inheritdoc />
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

        /// <inheritdoc />
        public abstract DecisionVector Operate(IEnumerable<DecisionVector> orderedVertices);

        #endregion

        protected static DecisionVector GetWorst(IEnumerable<DecisionVector> orderedVertices)
        {
            return orderedVertices.Last();
        }
        protected static DecisionVector GetBest(IEnumerable<DecisionVector> orderedVertices)
        {
            return orderedVertices.First();
        }
        protected static DecisionVector GetMean(IEnumerable<DecisionVector> orderedVertices)
        {
            // Average of all vertex locations except the worst.
            var allVertexVectorsExceptWorst = orderedVertices
                .Take(orderedVertices.Count() - 1)
                .Select(v => v.Vector.Select(d => (double)d));

            var centroid = allVertexVectorsExceptWorst.Aggregate(
                (a,b) => a.Select((x,i) => x + b.ElementAt(i)));

            var numDims = allVertexVectorsExceptWorst.Count();
            return DecisionVector.CreateFromArray(
                orderedVertices.First().GetDecisionSpace(),
                centroid.Select(x => x / numDims));
        }
    }
}
