using MathNet.Numerics.Random;

namespace Optimisation.Base.Variables
{
    /// <summary>
    /// The definition of a dimension, which is used for optimisation.
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// Determines if a value is supported by this variable.
        /// </summary>
        /// <param name="testValue">The value to test</param>
        /// <returns>True/false (whether the value is supported)</returns>
        /// <exception cref="System.FormatException">Thrown when object is of the wrong type.</exception>
        bool IsInBounds(object testValue);

        /// <summary>
        /// Gets a legal random number in this variable.
        /// </summary>
        /// <returns>A legal object</returns>
        object GetNextRandom(RandomSource rng);

        /// <summary>
        /// A string which describes the variable
        /// </summary>
        string Name { get; }
    }
}