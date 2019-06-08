using System.Collections.Generic;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Conversion
{
    /// <summary>
    /// The Converter converts Decision Vectors to reality and vice versa.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Converts real-world representation of a solution into a Decision Vector
        /// </summary>
        /// <param name="realityDefinition">Object representing reality</param>
        /// <returns>A Decision Vector</returns>
        DecisionVector ConvertToDV(object realityDefinition);
        
        /// <summary>
        /// Converts a Decision Vector into a real-world representation of the solution 
        /// </summary>
        /// <param name="decisionVector">Decision Vector</param>
        /// <returns>An object of the correct type</returns>
        object ConvertToReality(DecisionVector decisionVector);
    }
}