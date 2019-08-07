using MathNet.Numerics.Random;
using System.Collections.Generic;

namespace PopOptBox.Base.Variables
{
    /// <summary>
    /// The definition of a dimension, which is used for optimisation.
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// A string which describes the variable.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Determines if a value is supported by this variable.
        /// </summary>
        /// <param name="testValue">The value to test.</param>
        /// <returns><see langword="true"/> when the value is supported.</returns>
        /// <exception cref="System.InvalidCastException">Thrown when object is of the wrong type.</exception>
        bool IsInBounds(object testValue);

        /// <summary>
        /// Gets the legal value which is nearest to the provided value.
        /// </summary>
        /// <param name="testValue">The value to look near.</param>
        /// <returns>A legal value.</returns>
        object GetNearestLegalLocation(object testValue);

        /// <summary>
        /// Addition (or subtraction) operator that returns a new object within bounds. 
        /// </summary>
        /// <param name="value1">A valid value for this variable.</param>
        /// <param name="value2">An value to add (can be negative and invalid).</param>
        /// <returns>A legal value.</returns>
        object AddOrWrap(object value1, object value2);

        /// <summary>
        /// Gets a legal random number in this variable.
        /// </summary>
        /// <returns>A legal value.</returns>
        object GetNextRandom(RandomSource rng);

        /// <summary>
        /// Gets an array spaced evenly over the dimension (starting at the boundaries).
        /// </summary>
        /// <param name="numberOfLocations">The number of locations to return.</param>
        /// <returns>An array of legal objects.</returns>
        IEnumerable<object> GetSpacedArray(int numberOfLocations);

        /// <summary>
        /// Returns a string representation of a value for this variable.
        /// </summary>
        /// <param name="value">The value to be formatted.</param>
        /// <returns>A string.</returns>
        /// <exception cref="System.FormatException">Thrown when object is of the wrong type.</exception>
        string FormatAsString(object value);
    }
}