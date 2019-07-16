using MathNet.Numerics.Random;
using System;

namespace Optimisation.Base.Variables
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
        
        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Constructor specifies the dimension bounds
        /// </summary>
        /// <param name="lowerBound">Inclusive lower bound</param>
        /// <param name="upperBound">Exclusive upper bound</param>
        /// <param name="lowerBoundForGeneration">Inclusive lower bound for random number generation.</param>
        /// <param name="upperBoundForGeneration">Exclusive upper bound for random number generation.</param>
        /// <param name="name">A description for the variable, blank by default</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public VariableContinuous(
            double lowerBound = double.MinValue, 
            double upperBound = double.MaxValue,
            double lowerBoundForGeneration = -1e6,
            double upperBoundForGeneration = 1e6,
            string name = "")
        {
            if (upperBound <= lowerBound)
                throw new ArgumentOutOfRangeException(nameof(upperBound), 
                    "Variable range must be greater than zero.");
            
            if (upperBoundForGeneration <= lowerBoundForGeneration)
                throw new ArgumentOutOfRangeException(nameof(upperBoundForGeneration), 
                    "Variable generation range must be greater than zero.");
            
            this.lowerBound = lowerBound;
            this.lowerBoundForGeneration = lowerBoundForGeneration;
            this.upperBound = upperBound;
            this.upperBoundForGeneration = upperBoundForGeneration;
            Name = name;
        }

        /// <inheritdoc />
        public bool IsInBounds(object testValue)
        {
            var test = Convert.ToDouble(testValue);
            return test >= lowerBound && test < upperBound;
        }

        /// <summary>
        /// Implements random number generation for this variable
        /// </summary>
        /// <returns>A legal object (double).</returns>
        public object GetNextRandom(RandomSource rng)
        {
            return (rng.NextDouble() 
                    * (upperBoundForGeneration - lowerBoundForGeneration))
                   + lowerBoundForGeneration;
        }

        /// <summary>
        /// Returns a string representation of a value for this variable.
        /// </summary>
        /// <param name="value">The value to be formatted (should be a double)</param>
        /// <returns>A string</returns>
        /// <exception cref="System.FormatException">Thrown when object is of the wrong type.</exception>
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