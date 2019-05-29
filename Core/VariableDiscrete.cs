using System;

namespace Optimisation.Core
{
    /// <summary>
    /// Discrete dimension, represented by an integer type.
    /// Can also be used for categories (enums)
    /// </summary>
    public class VariableDiscrete : IVariable
    {
        private readonly int lowerBound;
        private readonly int upperBound;
        
        /// <summary>
        /// Constructor specifies the dimension bounds
        /// </summary>
        /// <param name="lowerBound">Smallest allowed value</param>
        /// <param name="upperBound">Largest allowed value</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public VariableDiscrete(
            int lowerBound = 0, 
            int upperBound = int.MaxValue)
        {
            if (upperBound <= lowerBound)
                throw new ArgumentOutOfRangeException(nameof(upperBound), 
                    "Variable range must be greater than zero.");
            
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        /// <inheritdoc />
        public bool IsInBounds(object testValue)
        {
            return (int)testValue >= lowerBound && (int)testValue <= upperBound;
        }
    }
}