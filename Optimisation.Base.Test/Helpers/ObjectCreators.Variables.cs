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
            var dv = vector.ToArray();
            return DecisionVector.CreateFromArray(
                GetDecisionSpace(
                    dv.Length,
                    dv.Min(),
                    dv.Max()), 
                dv);
        }
        
        internal static DecisionVector GetDecisionVector(IEnumerable<double> vector)
        {
            var dv = vector.ToArray();
            return DecisionVector.CreateFromArray(
                GetDecisionSpace(
                    dv.Count(),
                    dv.Min(),
                    dv.Max() + 0.01), 
                dv);
        }
    }
}