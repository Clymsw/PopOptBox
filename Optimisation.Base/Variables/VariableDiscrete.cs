using MathNet.Numerics.Random;
using System;

namespace Optimisation.Base.Variables
{
    /// <summary>
    /// Discrete dimension, represented by an integer type.
    /// Can also be used for categories (enums)
    /// </summary>
    public class VariableDiscrete : IVariable
    {
        private readonly int lowerBound;
        private readonly int lowerBoundForGeneration;
        private readonly int upperBound;
        private readonly int upperBoundForGeneration;

        /// <summary>
        /// A string which describes the variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lowerBound">Smallest allowed value.</param>
        /// <param name="upperBound">Largest allowed value.</param>
        /// <param name="lowerBoundForGeneration">Inclusive lower bound for random number generation.</param>
        /// <param name="upperBoundForGeneration">Exclusive upper bound for random number generation.</param>
        /// <param name="name">A description for the variable, blank by default.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the the bounds for the variable or the generation range indicate a zero or negative range.
        /// </exception>
        public VariableDiscrete(
            int lowerBound = 0, 
            int upperBound = int.MaxValue,
            int lowerBoundForGeneration = 0,
            int upperBoundForGeneration = 1000000,
            string name = "")
        {
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
            var test = Convert.ToInt32(testValue);
            return test >= lowerBound && test <= upperBound;
        }

        /// <summary>
        /// Implements random number generation for this variable.
        /// </summary>
        /// <returns>A legal object (int).</returns>
        public object GetNextRandom(RandomSource rng)
        {
            return rng.Next(lowerBoundForGeneration, upperBoundForGeneration);
        }

        /// <summary>
        /// Wrapped addition (or subtraction) operator for this variable.
        /// </summary>
        /// <param name="value1">A valid integer for this variable.</param>
        /// <param name="value2">An integer to add (can be negative and invalid).</param>
        /// <returns>A new integer, valid for this variable.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="value1"/> is not in bounds.</exception>
        public object AddOrWrap(object value1, object value2)
        {
            if (!IsInBounds(value1))
                throw new ArgumentOutOfRangeException(nameof(value1), "Value must be legal for this variable.");
            
            // Get remainder in the range [1, r+1]
            var remainder = ((int)value1 + (int)value2 - lowerBound + 1) % (upperBound - lowerBound + 1);
            return remainder <= 0 ? upperBound + remainder : lowerBound + remainder - 1;
        }
        
        /// <summary>
        /// Returns a string representation of a value for this variable.
        /// </summary>
        /// <param name="value">The value to be formatted (should be parsable as an integer)</param>
        /// <returns>A string</returns>
        /// <exception cref="System.FormatException">Thrown when object is of the wrong type or cannot be converted.</exception>
        public string FormatAsString(object value)
        {
            var converted = Convert.ToInt32(value);
            return converted.ToString("D", System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public override string ToString()
        {
            return $"{Name} [{lowerBound} - {upperBound}]";
        }

        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (!(obj is VariableDiscrete other))
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