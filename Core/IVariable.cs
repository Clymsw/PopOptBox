namespace Optimisation.Core
{
    /// <summary>
    /// The definition of a dimension, which is used for optimisation.
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// A function which determines if a value is supported by this variable.
        /// </summary>
        /// <param name="testValue">The value to test</param>
        /// <returns>True/false (whether the value is supported)</returns>
        bool IsInBounds(object testValue);
    }
}