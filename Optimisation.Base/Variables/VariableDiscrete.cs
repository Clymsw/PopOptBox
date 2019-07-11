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
        private readonly int upperBound;
        
        public string Name { get; }
        
        /// <summary>
        /// Constructor specifies the dimension bounds
        /// </summary>
        /// <param name="lowerBound">Smallest allowed value</param>
        /// <param name="upperBound">Largest allowed value</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public VariableDiscrete(
            int lowerBound = 0, 
            int upperBound = int.MaxValue,
            string name = "")
        {
            if (upperBound <= lowerBound)
                throw new ArgumentOutOfRangeException(nameof(upperBound), 
                    "Variable range must be greater than zero.");
            
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
            Name = name;
        }

        /// <inheritdoc />
        public bool IsInBounds(object testValue)
        {
            var test = System.Convert.ToInt32(testValue);
            return test >= lowerBound && test <= upperBound;
        }

        /// <summary>
        /// Implements random number generation for this variable
        /// </summary>
        /// <returns>A legal object (double).</returns>
        public object GetNextRandom(RandomSource rng)
        {
            return rng.Next(lowerBound - 1, upperBound + 1);
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