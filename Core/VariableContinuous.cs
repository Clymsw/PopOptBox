using System;

namespace Optimisation.Core
{
    /// <summary>
    /// Continuous dimension, represented by a double type.
    /// </summary>
    public class VariableContinuous : IVariable
    {
        private readonly double lowerBound;
        private readonly double upperBound;
        
        /// <summary>
        /// Constructor specifies the dimension bounds
        /// </summary>
        /// <param name="lowerBound">Inclusive lower bound</param>
        /// <param name="upperBound">Exclusive upper bound</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public VariableContinuous(
            double lowerBound = double.MinValue, 
            double upperBound = double.MaxValue)
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
            return (double)testValue >= lowerBound && (double)testValue < upperBound;
        }
    }
}