using System;

namespace Optimisation.Base.Variables
{
    /// <summary>
    /// Continuous dimension, represented by a double type.
    /// </summary>
    public class VariableContinuous : IVariable
    {
        private readonly double lowerBound;
        private readonly double upperBound;
        
        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Constructor specifies the dimension bounds
        /// </summary>
        /// <param name="lowerBound">Inclusive lower bound</param>
        /// <param name="upperBound">Exclusive upper bound</param>
        /// <param name="name">A description for the variable, blank by default</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public VariableContinuous(
            double lowerBound = double.MinValue, 
            double upperBound = double.MaxValue,
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
            var test = System.Convert.ToDouble(testValue);
            return test >= lowerBound && test < upperBound;
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