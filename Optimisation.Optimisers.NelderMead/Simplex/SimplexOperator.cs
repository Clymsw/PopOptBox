using System.Linq;
using System.Collections.Generic;
using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.NelderMead.Simplex
{
    public abstract class SimplexOperator : ISimplexOperator
    {
        #region Fields

        protected double coefficient;
        /// <summary>
        /// Variable affecting the behaviour of <see cref="Operate(IOrderedEnumerable{DecisionVector})"/>.
        /// </summary>
        public double Coefficient { get => GetCoefficient(); set => SetCoefficient(value); }

        public virtual double GetCoefficient() => coefficient;
        public abstract void SetCoefficient(double value);

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

        public abstract DecisionVector Operate(IOrderedEnumerable<DecisionVector> orderedVertices);

        #endregion

        protected DecisionVector GetWorst(IOrderedEnumerable<DecisionVector> orderedVertices)
        {
            return orderedVertices.Last();
        }
        protected DecisionVector GetBest(IOrderedEnumerable<DecisionVector> orderedVertices)
        {
            return orderedVertices.First();
        }
        protected DecisionVector GetMean(IOrderedEnumerable<DecisionVector> orderedVertices)
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
