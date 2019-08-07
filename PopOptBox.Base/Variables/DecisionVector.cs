using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PopOptBox.Base.Variables
{
    /// <summary>
    /// A Decision Vector is a particular point inside a decision space.
    /// In other words, a particular solution to an optimisation problem.
    /// </summary>
    public class DecisionVector : IReadOnlyCollection<object>
    {
        private readonly DecisionSpace decisionSpace;

        /// <summary>
        /// Definition of the solution for the optimiser.
        /// Valid in conjunction with the <see cref="DecisionSpace"/> stored inside.
        /// </summary>
        private readonly IReadOnlyList<object> vector;

        #region Constructor

        private DecisionVector(DecisionSpace decisionSpace,
            IEnumerable<object> values)
        {
            this.decisionSpace = decisionSpace;

            //Check that all values are sensible
            if (!decisionSpace.IsAcceptableDecisionVector(values))
                throw new ArgumentOutOfRangeException(nameof(values),
                    "These values are not accepted by the decision space");

            vector = values.ToArray();
        }

        /// <summary>
        /// Returns a new empty Decision Vector.
        /// </summary>
        /// <returns>A <see cref="DecisionVector"/> with no elements.</returns>
        public static DecisionVector CreateForEmpty()
        {
            return new DecisionVector(DecisionSpace.CreateForEmpty(), new List<object>());
        }

        /// <summary>
        /// Static constructor which takes a comma-separated list of values.
        /// Useful when the decision space is heteromorphic.
        /// </summary>
        /// <param name="decisionSpace">The decision space definition</param>
        /// <param name="values">The points inside that decision space defining this solution</param>
        /// <returns>A new Decision Vector</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> are not valid.</exception>
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
        /// <returns>A new Decision Vector</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> are not valid.</exception>
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
        /// <returns>A new Decision Vector</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> are not valid.</exception>
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
        /// <returns>A new Decision Vector</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> are not valid.</exception>
        public static DecisionVector CreateFromArray(DecisionSpace decisionSpace,
            IEnumerable<double> values)
        {
            return new DecisionVector(decisionSpace, values.Cast<object>().ToArray());
        }

        #endregion

        /// <summary>
        /// An indexer to allow direct indexation to the DecisionVector class, which will return the object of interest.
        /// </summary>
        /// <param name="index">Element index.</param>
        /// <returns>An object.</returns>
        public object this[int index] => vector[index];
        
        #region Decision Space

        /// <summary>
        /// Gets the <see cref="DecisionSpace"/> of this Decision Vector.
        /// </summary>
        /// <returns>A <see cref="DecisionSpace"/>.</returns>
        public DecisionSpace GetDecisionSpace()
        {
            return decisionSpace;
        }

        /// <summary>
        /// Get only those elements of the Decision Vector which are continuous.
        /// </summary>
        /// <returns>New Decision Vector with only those elements</returns>
        public DecisionVector GetContinuousElements()
        {
            return CreateFromArray(
                new DecisionSpace(decisionSpace
                    .Where(d => d.GetType() == typeof(VariableContinuous))),
                vector.Where(
                        (v, i) => decisionSpace.ElementAt(i).GetType() == typeof(VariableContinuous))
                    .Cast<double>());
        }
        
        /// <summary>
        /// Get only those elements of the Decision Vector which are discrete.
        /// </summary>
        /// <returns>New Decision Vector with only those elements</returns>
        public DecisionVector GetDiscreteElements()
        {
            return CreateFromArray(
                new DecisionSpace(decisionSpace
                        .Where(d => d.GetType() == typeof(VariableDiscrete))),
                vector.Where(
                        (v, i) => decisionSpace.ElementAt(i).GetType() == typeof(VariableDiscrete))
                    .Cast<int>());
        }

        #endregion

        #region Operators

        /// <summary>
        /// Gets the point-wise difference between two decision vectors.
        /// </summary>
        /// <param name="v1">A <see cref="DecisionVector"/>.</param>
        /// <param name="v2">Another <see cref="DecisionVector"/> to subtract.</param>
        /// <returns>An array of values representing the difference.</returns>
        public static IEnumerable<double> operator -(DecisionVector v1, DecisionVector v2)
        {
            if (!v1.decisionSpace.Equals(v2.decisionSpace))
                throw new ArgumentException("Decision vectors for comparison must have identical decision spaces.");

            var difference = new double[v1.vector.Count];
            for (var i = 0; i < v1.vector.Count; i++)
            {
                difference[i] = (double) v1.vector[i] - (double) v2.vector[i];
            }

            return difference;
        }

        /// <summary>
        /// Gets the point-wise sum of two decision vectors.
        /// </summary>
        /// <param name="v1">A <see cref="DecisionVector"/>.</param>
        /// <param name="v2">Another <see cref="DecisionVector"/> to add.</param>
        /// <returns>An array of values representing the sum.</returns>
        public static IEnumerable<double> operator +(DecisionVector v1, DecisionVector v2)
        {
            if (!v1.decisionSpace.Equals(v2.decisionSpace))
                throw new ArgumentException("Decision vectors for comparison must have identical decision spaces.");

            var sum = new double[v1.vector.Count];
            for (var i = 0; i < v1.vector.Count; i++)
            {
                sum[i] = (double) v1.vector[i] + (double) v2.vector[i];
            }

            return sum;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return decisionSpace.FormatAsString(vector);
        }

        #endregion

        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (!(obj is DecisionVector other))
                return false;

            return decisionSpace.Equals(other.decisionSpace) &&
                vector.SequenceEqual(other.vector);
        }
        
        public override int GetHashCode()
        {
            return new
            {
                decisionSpace,
                Vector = vector
            }.GetHashCode();
        }

        #endregion
        
        #region Implementation of IReadOnlyCollection<out object>

        public int Count => vector.Count;
        
        #endregion
        
        #region Implementation of IEnumerable
        
        public IEnumerator<object> GetEnumerator()
        {
            return vector.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) vector).GetEnumerator();
        }
        
        #endregion
    }
}