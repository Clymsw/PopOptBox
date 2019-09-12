using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PopOptBox.Base.Variables
{
    /// <summary>
    /// Continuous dimension, represented by a double type.
    /// </summary>
    public class VariableContinuous : IVariable
    {
        private readonly double lowerBound;
        private readonly double lowerBoundForGeneration;
        private readonly double upperBound;
        private readonly double upperBoundForGeneration;

        /// <summary>
        /// A string which describes the variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lowerBound">Inclusive lower bound.</param>
        /// <param name="upperBound">Exclusive upper bound.</param>
        /// <param name="lowerBoundForGeneration">Inclusive lower bound for random number generation.</param>
        /// <param name="upperBoundForGeneration">Exclusive upper bound for random number generation.</param>
        /// <param name="name">A description for the variable, blank by default.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the the bounds for the variable or the generation range indicate a zero or negative range.
        /// </exception>
        public VariableContinuous(
            double lowerBound = double.MinValue, 
            double upperBound = double.MaxValue,
            double lowerBoundForGeneration = -1e6,
            double upperBoundForGeneration = 1e6,
            string name = "")
        {
            if (double.IsNegativeInfinity(lowerBound))
                throw new ArgumentOutOfRangeException(nameof(lowerBound), 
                    "Variable range cannot be infinite, use double.MinValue instead (this is default).");
            
            if (double.IsPositiveInfinity(upperBound))
                throw new ArgumentOutOfRangeException(nameof(upperBound), 
                    "Variable range cannot be infinite, use double.MaxValue instead (this is default).");
            
            if (upperBound <= lowerBound)
                throw new ArgumentOutOfRangeException(nameof(upperBound), 
                    "Variable range must be greater than zero.");
            
            if (upperBoundForGeneration <= lowerBoundForGeneration)
                throw new ArgumentOutOfRangeException(nameof(upperBoundForGeneration), 
                    "Variable generation range must be greater than zero.");
            
            this.lowerBound = lowerBound;
            this.lowerBoundForGeneration = Math.Max(lowerBound, lowerBoundForGeneration);
            this.upperBound = upperBound;
            this.upperBoundForGeneration = Math.Min(upperBound, upperBoundForGeneration);
            Name = name;
        }

        /// <summary>
        /// Determines if a value is supported by this variable.
        /// </summary>
        /// <param name="testValue">The value to test.</param>
        /// <returns><see langword="true"/> when the value is supported.</returns>
        /// <exception cref="System.FormatException">Thrown when object is of the wrong type or cannot be converted.</exception>
        public bool IsInBounds(object testValue)
        {
            var test = Convert.ToDouble(testValue);
            return test >= lowerBound && test < upperBound;
        }

        /// <summary>
        /// Gets the legal value which is nearest to the provided value.
        /// </summary>
        /// <param name="testValue">The value to look near.</param>
        /// <returns>A legal value (double).</returns>
        public object GetNearestLegalLocation(object testValue)
        {
            if (IsInBounds(testValue))
                return testValue;

            return Convert.ToDouble(testValue) >= upperBound
                ? upperBound - 1e-12 // upper bound itself is illegal
                : lowerBound;
        }

        /// <summary>
        /// Implements random number generation for this variable
        /// </summary>
        /// <returns>A legal value (double).</returns>
        public object GetNextRandom(RandomSource rng)
        {
            return (rng.NextDouble() 
                    * (upperBoundForGeneration - lowerBoundForGeneration))
                   + lowerBoundForGeneration;
        }
        
        /// <summary>
        /// Wrapped addition (or subtraction) operator for this variable.
        /// </summary>
        /// <param name="value1">A valid double for this variable.</param>
        /// <param name="value2">An double to add (can be negative and invalid).</param>
        /// <returns>A new double, valid for this variable.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="value1"/> is not in bounds.</exception>
        public object AddOrWrap(object value1, object value2)
        {
            if (!IsInBounds(value1))
                throw new ArgumentOutOfRangeException(nameof(value1), "Value must be legal for this variable.");

            if (lowerBound == double.MinValue || upperBound == double.MaxValue)
                return (double)value1 + (double)value2;

            // Get remainder in the range [0, r]
            var remainder = ((double)value1 + (double)value2 - lowerBound) % (upperBound - lowerBound);
            return remainder <= 0 ? upperBound + remainder : lowerBound + remainder;
        }

        /// <summary>
        /// Gets an array of evenly-spaced values across the dimension (starting at the boundaries).
        /// </summary>
        /// <param name="numberOfLocations">The number of locations to return.</param>
        /// <returns>A list of doubles.</returns>
        public IEnumerable<object> GetSpacedArray(int numberOfLocations)
        {
            return Enumerable.Range(0, numberOfLocations)
                .Select(i => (object)(lowerBound + (i * (upperBound - lowerBound) / (numberOfLocations - 1))));
        }

        /// <summary>
        /// Returns a string representation of a value for this variable.
        /// </summary>
        /// <param name="value">The value to be formatted (should be a double)</param>
        /// <returns>A string</returns>
        /// <exception cref="System.FormatException">Thrown when object is of the wrong type or cannot be converted.</exception>
        public string FormatAsString(object value)
        {
            var converted = Convert.ToDouble(value);
            return converted.ToString("G3", System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public override string ToString()
        {
            return $"{Name} [{lowerBound} - {upperBound}]";
        }

        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (!(obj is VariableContinuous other))
                return false;

            return lowerBound.Equals(other.lowerBound) && 
                upperBound.Equals(other.upperBound);
        }

        public override int GetHashCode()
        {
            return new
            {
                lowerBound,
                upperBound
            }.GetHashCode();
        }

        #endregion
    }
}