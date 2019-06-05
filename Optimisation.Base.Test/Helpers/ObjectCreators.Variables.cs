using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Test.Helpers
{
    internal static partial class ObjectCreators
    {
        internal static DecisionSpace GetDecisionSpace(int dims, int lowerBound, int upperBound)
        {
            return DecisionSpace.CreateForUniformIntArray(
                dims, lowerBound, upperBound);
        }
        
        internal static DecisionSpace GetDecisionSpace(int dims, double lowerBound, double upperBound)
        {
            return DecisionSpace.CreateForUniformDoubleArray(
                dims, lowerBound, upperBound);
        }

        internal static DecisionVector GetDecisionVector(IEnumerable<int> vector)
        {
            return DecisionVector.CreateFromArray(
                GetDecisionSpace(vector.Count(), vector.Min(), vector.Max()), 
                vector);
        }
        
        internal static DecisionVector GetDecisionVector(IEnumerable<double> vector)
        {
            return DecisionVector.CreateFromArray(
                GetDecisionSpace(vector.Count(), vector.Min(), vector.Max() + 0.01), 
                vector);
        }
    }
}