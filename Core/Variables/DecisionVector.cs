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
            object[] values)
        {
            this.decisionSpace = decisionSpace;

            //Check that all values are sensible
            if (!decisionSpace.IsAcceptableDecisionVector(values))
                throw new ArgumentOutOfRangeException(nameof(values),
                    "Vector values are not accepted by the decision space");

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

        // TODO: Extend this to retrieve the elements of each type, based on the decision space.
        
        
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