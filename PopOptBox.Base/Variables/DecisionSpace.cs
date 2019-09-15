using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PopOptBox.Base.Variables
{
    /// <summary>
    /// An array of dimensions which together specify the N-D space for optimisation.
    /// Immutable by design.
    /// </summary>
    public class DecisionSpace : IReadOnlyCollection<IVariable>
    {
        /// <summary>
        /// The list of <see cref="IVariable"/> definitions for the dimensions.
        /// </summary>
        private readonly IReadOnlyList<IVariable> dimensions;

        #region Constructors
        
        /// <summary>
        /// The constructor can be used for mixed arrays.
        /// </summary>
        /// <remarks>
        /// For uniform arrays, it is easier to use static constructors:
        /// <see cref="CreateForUniformDoubleArray"/> or <see cref="CreateForUniformIntArray"/>
        /// </remarks>
        /// <param name="decisionSpace">The array of <see cref="IVariable"/>s.</param>
        public DecisionSpace(IEnumerable<IVariable> decisionSpace)
        {
            dimensions = decisionSpace.ToArray();
        }

        /// <summary>
        /// Returns an empty Decision Space.
        /// </summary>
        /// <returns>A <see cref="DecisionSpace"/> with no elements.</returns>
        public static DecisionSpace CreateForEmpty()
        {
            return new DecisionSpace(new List<IVariable>());
        }

        /// <summary>
        /// Static constructor for a hypercube which will take discrete values.
        /// <seealso cref="VariableDiscrete"/>.
        /// </summary>
        /// <param name="numDimensions">Number of dimensions</param>
        /// <param name="lowerBound">The lower bound for each dimension</param>
        /// <param name="upperBound">The upper bound for each dimension</param>
        /// <param name="lowerBoundForGeneration">The lower bound of RNG for each dimension</param>
        /// <param name="upperBoundForGeneration">The upper bound of RNG for each dimension</param>
        /// <returns>A new decision space</returns>
        public static DecisionSpace CreateForUniformIntArray(int numDimensions, 
            int lowerBound, int upperBound, 
            int lowerBoundForGeneration = -1000000, int upperBoundForGeneration = 1000000)
        {
            return new DecisionSpace(
                Enumerable.Range(0, numDimensions)
                    .Select(i => new VariableDiscrete(
                        lowerBound, upperBound, 
                        lowerBoundForGeneration, upperBoundForGeneration,
                        $"Dimension {i + 1}"))
                    .ToArray());
        }

        /// <summary>
        /// Static constructor for a hypercube which will take continuous values.
        /// <seealso cref="VariableContinuous"/>.
        /// </summary>
        /// <param name="numDimensions">Number of dimensions</param>
        /// <param name="lowerBound">The lower bound for each dimension</param>
        /// <param name="upperBound">The upper bound for each dimension</param>
        /// <param name="lowerBoundForGeneration">The lower bound of RNG for each dimension</param>
        /// <param name="upperBoundForGeneration">The upper bound of RNG for each dimension</param>
        /// <returns>A new decision space</returns>
        public static DecisionSpace CreateForUniformDoubleArray(int numDimensions, 
            double lowerBound, double upperBound, 
            double lowerBoundForGeneration = -1e6, double upperBoundForGeneration = 1e6)
        {
            return new DecisionSpace(
                Enumerable.Range(0, numDimensions)
                    .Select(i => new VariableContinuous(
                        lowerBound, upperBound, 
                        lowerBoundForGeneration, upperBoundForGeneration,
                        $"Dimension {i + 1}"))
                    .ToArray());
        }

        #endregion
        
        /// <summary>
        /// An indexer to allow direct indexation to the DecisionSpace class, which will return the <see cref="IVariable"/> of interest.
        /// </summary>
        /// <param name="index">Element index.</param>
        /// <returns>An <see cref="IVariable"/>.</returns>
        public IVariable this[int index] => dimensions[index];
        
        /// <summary>
        /// Helper function, which can be used to check an array for validity,
        /// before using it to construct a <see cref="DecisionVector"/>.
        /// </summary>
        /// <param name="vector">The array of values.</param>
        /// <returns>True (acceptable) or false (not acceptable).</returns>
        public bool IsAcceptableDecisionVector(IEnumerable<object> vector)
        {
            var elements = vector as object[] ?? vector.ToArray();
            
            if (elements.Length != dimensions.Count)
                return false;

            var acceptable = true;
            for (var i = 0; i < elements.Length; i++)
            {
                try
                {
                    var ok = dimensions.ElementAt(i).IsInBounds(elements.ElementAt(i));
                    acceptable &= ok;
                }
                catch
                {
                    acceptable = false;
                    break;
                }
            }
            return acceptable;
        }

        /// <summary>
        /// Gets the legal location which is nearest to the provided location.
        /// </summary>
        /// <param name="vector">An array of objects matching (but not necessarily legal in) the decision space.</param>
        /// <returns>A new object array representing a location.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the location provided is of the wrong length.</exception>
        /// <exception cref="FormatException">Thrown when the location provided has incorrect types.</exception>
        public object[] GetNearestLegalLocation(IEnumerable<object> vector)
        {
            var elements = vector as object[] ?? vector.ToArray();
            
            if (elements.Length != dimensions.Count)
                throw new ArgumentOutOfRangeException(nameof(vector),
                    "Vector is of the wrong length.");
            
            return elements.Select((v, i) => dimensions.ElementAt(i).GetNearestLegalLocation(v)).ToArray();
        }
        
        /// <summary>
        /// Gets the string representation of a particular vector, according to the <see cref="IVariable"/> definitions of the decision space.
        /// </summary>
        /// <param name="vector">The array of values to format.</param>
        /// <returns>A string.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the vector is the wrong length. Use <seealso cref="IsAcceptableDecisionVector(IEnumerable{object})"/> first.</exception>
        public string FormatAsString(IEnumerable<object> vector)
        {
            return string.Join(" - ", vector.Select((d, i) => dimensions.ElementAt(i).FormatAsString(d)));
        }

        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (!(obj is DecisionSpace other))
                return false;

            return dimensions.SequenceEqual(other.dimensions);
        }
        
        public override int GetHashCode()
        {
            return new
            {
                Dimensions = dimensions
            }.GetHashCode();
        }

        #endregion
        
        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) dimensions).GetEnumerator();
        }
        
        public IEnumerator<IVariable> GetEnumerator()
        {
            return dimensions.GetEnumerator();
        }
        
        #endregion

        #region Implementation of IReadOnlyCollection<out IVariable>
        
        public int Count => dimensions.Count;
        
        #endregion
    }
}