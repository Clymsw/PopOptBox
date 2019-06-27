using System.Linq;
using System.Collections.Generic;
using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.NelderMead.Simplices
{
    /// <inheritdoc />
    public abstract class SimplexOperator : ISimplexOperator
    {
        #region Fields

        private double coefficient;
        /// <summary>
        /// Variable affecting the behaviour of <see cref="Operate(IEnumerable{DecisionVector})"/>.
        /// </summary>
        public double Coefficient
        {
            get => coefficient;
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
        public SimplexOperator(double coefficient)
        {
            Coefficient = coefficient;
        }

        #endregion

        #region Implement Interface

        /// <inheritdoc />
        public abstract DecisionVector Operate(IEnumerable<DecisionVector> orderedVertices);

        #endregion

        protected DecisionVector GetWorst(IEnumerable<DecisionVector> orderedVertices)
        {
            return orderedVertices.Last();
        }
        protected DecisionVector GetBest(IEnumerable<DecisionVector> orderedVertices)
        {
            return orderedVertices.First();
        }
        protected DecisionVector GetMean(IEnumerable<DecisionVector> orderedVertices)
        {
            // Average of all vertex locations except the worst.
            var allVertexVectorsExceptWorst = orderedVertices
                .Take(orderedVertices.Count() - 1)
                .Select(v => v.Vector.Select(d => (double)d));

            var centroid = allVertexVectorsExceptWorst.Aggregate(
                (a,b) => a.Select((x,i) => x + b.ElementAt(i)));

            return DecisionVector.CreateFromArray(
                orderedVertices.First().GetDecisionSpace(),
                centroid.Select(x => x / allVertexVectorsExceptWorst.Count()));
        }
    }
}
