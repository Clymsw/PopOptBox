using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Base.Variables
{
    /// <summary>
    /// An array of dimensions which together specify the N-D space for optimisation.
    /// Immutable by design.
    /// </summary>
    public class DecisionSpace
    {
        /// <summary>
        /// The list of <see cref="IVariable"/> definitions for the dimensions.
        /// </summary>
        public readonly IReadOnlyList<IVariable> Dimensions;

        #region Constructor
        
        /// <summary>
        /// The constructor can be used for mixed arrays.
        /// For uniform arrays, it is easier to use static constructors:
        /// <see cref="CreateForUniformDoubleArray"/> and <see cref="CreateForUniformIntArray"/>
        /// </summary>
        /// <param name="decisionSpace">The array of <see cref="IVariable"/>s.</param>
        public DecisionSpace(IEnumerable<IVariable> decisionSpace)
        {
            Dimensions = decisionSpace.ToArray();
        }

        /// <summary>
        /// Returns an empty Decision Space.
        /// </summary>
        /// <returns>A <see cref="DecisionSpace"/> with no elements.</returns>
        public static DecisionSpace CreateForEmpty()
        {
            return new DecisionSpace(new List<IVariable>());
        }

        /// <summary>
        /// Static constructor for a hypercube which will take discrete values.
        /// <seealso cref="VariableDiscrete"/>.
        /// </summary>
        /// <param name="numDimensions">Number of dimensions</param>
        /// <param name="lowerBound">The lower bound for each dimension</param>
        /// <param name="upperBound">The upper bound for each dimension</param>
        /// <param name="lowerBoundForGeneration">The lower bound of RNG for each dimension</param>
        /// <param name="upperBoundForGeneration">The upper bound of RNG for each dimension</param>
        /// <returns>A new decision space</returns>
        public static DecisionSpace CreateForUniformIntArray(int numDimensions, 
            int lowerBound, int upperBound, 
            int lowerBoundForGeneration = -1000000, int upperBoundForGeneration = 1000000)
        {
            return new DecisionSpace(
                Enumerable.Range(0, numDimensions)
                    .Select(i => new VariableDiscrete(
                        lowerBound, upperBound, 
                        lowerBoundForGeneration, upperBoundForGeneration,
                        $"Dimension {i + 1}"))
                    .ToArray());
        }

        /// <summary>
        /// Static constructor for a hypercube which will take continuous values.
        /// <seealso cref="VariableContinuous"/>.
        /// </summary>
        /// <param name="numDimensions">Number of dimensions</param>
        /// <param name="lowerBound">The lower bound for each dimension</param>
        /// <param name="upperBound">The upper bound for each dimension</param>
        /// <param name="lowerBoundForGeneration">The lower bound of RNG for each dimension</param>
        /// <param name="upperBoundForGeneration">The upper bound of RNG for each dimension</param>
        /// <returns>A new decision space</returns>
        public static DecisionSpace CreateForUniformDoubleArray(int numDimensions, 
            double lowerBound, double upperBound, 
            double lowerBoundForGeneration = -1e6, double upperBoundForGeneration = 1e6)
        {
            return new DecisionSpace(
                Enumerable.Range(0, numDimensions)
                    .Select(i => new VariableContinuous(
                        lowerBound, upperBound, 
                        lowerBoundForGeneration, upperBoundForGeneration,
                        $"Dimension {i + 1}"))
                    .ToArray());
        }

        #endregion
        
        /// <summary>
        /// Helper function, which can be used to check an array for validity,
        /// before using it to construct a <see cref="DecisionVector"/>.
        /// </summary>
        /// <param name="vector">The array of values.</param>
        /// <returns>True (acceptable) or false (not acceptable).</returns>
        public bool IsAcceptableDecisionVector(IEnumerable<object> vector)
        {
            var acceptable = true;
            for (var i = 0; i < vector.Count(); i++)
            {
                try
                {
                    var ok = Dimensions.ElementAt(i).IsInBounds(vector.ElementAt(i));
                    acceptable &= ok;
                }
                catch
                {
                    acceptable = false;
                    break;
                }
            }
            return acceptable;
        }

        public string FormatAsString(IEnumerable<object> vector)
        {
            return string.Join(" - ", vector.Select((d, i) => Dimensions.ElementAt(i).FormatAsString(d)));
        }

        #region Equals, GetHashCode
        
        public override bool Equals(object obj)
        {
            if (!(obj is DecisionSpace other))
                return false;

            return Dimensions.SequenceEqual(other.Dimensions);
        }
        
        public override int GetHashCode()
        {
            return new
            {
                Dimensions
            }.GetHashCode();
        }

        #endregion
    }
}