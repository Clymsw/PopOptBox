using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.NelderMead.Simplex
{
    public class ReflectExpandContract : SimplexOperator
    {
        public ReflectExpandContract(double coefficient) : base(coefficient)
        {
        }

        protected override bool CheckCoefficientAcceptable(double value)
        {
            if (value > -1)
                return true;
            else
                throw new System.ArgumentOutOfRangeException(nameof(value), value,
                    "Coefficient must be greater than -1");
        }

        public override DecisionVector Operate(IEnumerable<DecisionVector> orderedVertices)
        {
            var worst = GetWorst(orderedVertices);
            var mean = GetMean(orderedVertices);

            var newLocation = mean.Vector.Select(
                (d, i) => (1 + Coefficient) * (double)d -
                Coefficient * (double)worst.Vector[i]);

            return DecisionVector.CreateFromArray(
                worst.GetDecisionSpace(),
                newLocation);
        }
    }
}
