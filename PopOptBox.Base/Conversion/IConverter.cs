using PopOptBox.Base.Variables;

namespace PopOptBox.Base.Conversion
{
    /// <summary>
    /// The Converter converts Decision Vectors to reality and vice versa.
    /// </summary>
    /// <typeparam name="TReality">The type of the object representing reality.</typeparam>
    public interface IConverter<TReality>
    {
        /// <summary>
        /// Converts real-world representation of a solution into a Decision Vector.
        /// </summary>
        /// <param name="realityDefinition">Object representing reality</param>
        /// <returns>A <see cref="DecisionVector"/>.</returns>
        DecisionVector ConvertToDv(TReality realityDefinition);

        /// <summary>
        /// Converts a Decision Vector into a real-world representation of the solution.
        /// </summary>
        /// <param name="decisionVector">The <see cref="DecisionVector"/> of the <see cref="PopOptBox.Base.Management.Individual"/>.</param>
        /// <returns>An object of the correct type, representing reality.</returns>
        TReality ConvertToReality(DecisionVector decisionVector);
    }
}