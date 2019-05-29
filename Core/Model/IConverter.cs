using System.Collections.Generic;

namespace Optimisation.Core.Model
{
    /// <summary>
    /// The Converter converts Decision Vectors to reality and vice versa.
    /// </summary>
    /// <typeparam name="TDecVec">Class of decision vector, e.g. int</typeparam>
    /// <typeparam name="TReality">Class of input to evaluators, e.g. double</typeparam>
    public interface IConverter<TDecVec, TReality>
    {
        /// <summary>
        /// Converts real-world representation of a solution into a Decision Vector
        /// </summary>
        /// <param name="realityDefinition">Object representing reality</param>
        /// <returns>A Decision Vector</returns>
        IEnumerable<TDecVec> ConvertToDV(TReality realityDefinition);
        
        /// <summary>
        /// Converts a Decision Vector into a real-world representation of the solution 
        /// </summary>
        /// <param name="decisionVector">Decision Vector</param>
        /// <returns>An object of the correct type</returns>
        TReality ConvertToReality(IReadOnlyList<TDecVec> decisionVector);
    }
}