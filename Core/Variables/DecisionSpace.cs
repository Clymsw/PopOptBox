using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Base.Variables
{
    /// <summary>
    /// TODO!
    /// </summary>
    public class DecisionSpace
    {
        public readonly IReadOnlyList<IVariable> Dimensions;

        public DecisionSpace(IEnumerable<IVariable> decisionSpace)
        {
            Dimensions = decisionSpace.ToArray();
        }

        public static DecisionSpace CreateForUniformIntArray(int numDimensions, 
            int lowerBound, int upperBound)
        {
            return new DecisionSpace(
                Enumerable.Range(0, numDimensions)
                    .Select(i => new VariableDiscrete(lowerBound, upperBound))
                    .ToArray());
        }
        
        public static DecisionSpace CreateForUniformDoubleArray(int numDimensions, 
            double lowerBound, double upperBound)
        {
            return new DecisionSpace(
                Enumerable.Range(0, numDimensions)
                    .Select(i => new VariableContinuous(lowerBound, upperBound))
                    .ToArray());
        }

        public bool IsAcceptableDecisionVector(IEnumerable<object> vector)
        {
            bool acceptable = true;
            for (int i = 0; i < vector.Count(); i++)
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