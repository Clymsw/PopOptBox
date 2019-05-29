using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Core.Population
{
    public class DecisionSpace
    {
        public readonly IReadOnlyList<IVariable> Dimensions;

        public DecisionSpace(IEnumerable<IVariable> decisionSpace)
        {
            Dimensions = decisionSpace.ToArray();
        }

        public static DecisionSpace CreateForIntArray(int numDimensions, 
            int lowerBound, int upperBound)
        {
            return new DecisionSpace(
                Enumerable.Range(0, numDimensions)
                    .Select(i => new VariableDiscrete(lowerBound, upperBound))
                    .ToArray());
        }
        
        public static DecisionSpace CreateForDoubleArray(int numDimensions, 
            double lowerBound, double upperBound)
        {
            return new DecisionSpace(
                Enumerable.Range(0, numDimensions)
                    .Select(i => new VariableContinuous(lowerBound, upperBound))
                    .ToArray());
        }

        #region Equals, GetHashCode
        
        public override bool Equals(object obj)
        {
            if (!(obj is DecisionSpace other))
                return false;

            return Dimensions.Equals(other.Dimensions);
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