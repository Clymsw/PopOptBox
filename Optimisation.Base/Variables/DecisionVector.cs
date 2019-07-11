using System;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Base.Variables
{
    /// <summary>
    /// A decision vector is a particular point inside a decision space.
    /// In other words, a particular solution to an optimisation problem.
    /// </summary>
    public class DecisionVector
    {
        private readonly DecisionSpace decisionSpace;

        /// <summary>
        /// Definition of the solution for the optimiser.
        /// Valid in conjunction with the <see cref="DecisionSpace"/> stored inside.
        /// </summary>
        public readonly IReadOnlyList<object> Vector;

        #region Constructor

        private DecisionVector(DecisionSpace decisionSpace,
            IEnumerable<object> values)
        {
            this.decisionSpace = decisionSpace;

            //Check that all values are sensible
            if (!decisionSpace.IsAcceptableDecisionVector(values))
                throw new ArgumentOutOfRangeException(nameof(values),
                    "These values are not accepted by the decision space");

            Vector = values.ToArray();
        }

        /// <summary>
        /// Static constructor which takes a comma-separated list of values.
        /// Useful when the decision space is heteromorphic.
        /// </summary>
        /// <param name="decisionSpace">The decision space definition</param>
        /// <param name="values">The points inside that decision space defining this solution</param>
        /// <returns>A new decision vector</returns>
        public static DecisionVector CreateFromItems(DecisionSpace decisionSpace,
            params object[] values)
        {
            return new DecisionVector(decisionSpace, values);
        }

        /// <summary>
        /// Static constructor which takes an array of objects.
        /// </summary>
        /// <param name="decisionSpace">The decision space definition</param>
        /// <param name="values">The points inside that decision space defining this solution</param>
        /// <returns>A new decision vector</returns>
        public static DecisionVector CreateFromArray(DecisionSpace decisionSpace,
            IEnumerable<object> values)
        {
            return new DecisionVector(decisionSpace, values.ToArray());
        }

        /// <summary>
        /// Static constructor which takes an array of integers.
        /// Useful when the decision space is homomorphic and discrete.
        /// </summary>
        /// <param name="decisionSpace">The decision space definition</param>
        /// <param name="values">The points inside that decision space defining this solution</param>
        /// <returns>A new decision vector</returns>
        public static DecisionVector CreateFromArray(DecisionSpace decisionSpace,
            IEnumerable<int> values)
        {
            return new DecisionVector(decisionSpace, values.Cast<object>().ToArray());
        }

        /// <summary>
        /// Static constructor which takes an array of integers.
        /// Useful when the decision space is homomorphic and continuous.
        /// </summary>
        /// <param name="decisionSpace">The decision space definition</param>
        /// <param name="values">The points inside that decision space defining this solution</param>
        /// <returns>A new decision vector</returns>
        public static DecisionVector CreateFromArray(DecisionSpace decisionSpace,
            IEnumerable<double> values)
        {
            return new DecisionVector(decisionSpace, values.Cast<object>().ToArray());
        }

        #endregion

        #region Decision Space

        /// <summary>
        /// Gets the <see cref="DecisionSpace"/> of this decision vector.
        /// </summary>
        /// <returns>A <see cref="DecisionSpace"/>.</returns>
        public DecisionSpace GetDecisionSpace()
        {
            return decisionSpace;
        }

        /// <summary>
        /// Get only those elements of the decision vector which are continuous
        /// </summary>
        /// <returns>New Decision Vector with only those elements</returns>
        public DecisionVector GetContinuousElements()
        {
            return CreateFromArray(
                new DecisionSpace(decisionSpace.Dimensions
                    .Where(d => d.GetType() == typeof(VariableContinuous))),
                Vector.Where(
                        (v, i) => decisionSpace.Dimensions.ElementAt(i).GetType() == typeof(VariableContinuous))
                    .Cast<double>());
        }
        
        /// <summary>
        /// Get only those elements of the decision vector which are discrete
        /// </summary>
        /// <returns>New Decision Vector with only those elements</returns>
        public DecisionVector GetDiscreteElements()
        {
            return CreateFromArray(
                new DecisionSpace(decisionSpace.Dimensions
                        .Where(d => d.GetType() == typeof(VariableDiscrete))),
                Vector.Where(
                        (v, i) => decisionSpace.Dimensions.ElementAt(i).GetType() == typeof(VariableDiscrete))
                    .Cast<int>());
        }

        #endregion

        #region Operators

        public static IEnumerable<double> operator -(DecisionVector v1, DecisionVector v2)
        {
            if (!v1.decisionSpace.Equals(v2.decisionSpace))
                throw new ArgumentException("Decision vectors for comparison must have identical decision spaces.");

            var difference = new double[v1.Vector.Count];
            for (var i = 0; i < v1.Vector.Count; i++)
            {
                difference[i] = (double) v1.Vector[i] - (double) v2.Vector[i];
            }

            return difference;
        }
        
        public static IEnumerable<double> operator +(DecisionVector v1, DecisionVector v2)
        {
            if (!v1.decisionSpace.Equals(v2.decisionSpace))
                throw new ArgumentException("Decision vectors for comparison must have identical decision spaces.");

            var sum = new double[v1.Vector.Count];
            for (var i = 0; i < v1.Vector.Count; i++)
            {
                sum[i] = (double) v1.Vector[i] + (double) v2.Vector[i];
            }

            return sum;
        }

        #endregion
        
        #region Equals, GetHashCode
        
        public override bool Equals(object obj)
        {
            if (!(obj is DecisionVector other))
                return false;

            return decisionSpace.Equals(other.decisionSpace) &&
                Vector.SequenceEqual(other.Vector);
        }
        
        public override int GetHashCode()
        {
            return new
            {
                decisionSpace,
                Vector
            }.GetHashCode();
        }

        #endregion
    }
}