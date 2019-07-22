using System.Collections.Generic;
using Optimisation.Base.Conversion;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Test.Helpers
{
    internal static partial class ObjectCreators
    {
        internal const string Definition_Key = "TestDefinition";
        
        internal class ModelMock : Model<double>
        {
            private readonly DecisionVector decisionVector;
        
            public ModelMock(DecisionVector decisionVector, IConverter<double> converter) : 
                base(converter, Definition_Key)
            {
                this.decisionVector = decisionVector;
            }

            protected override DecisionVector CreateNewIndividual()
            {
                return decisionVector;
            }
        }
        
        internal class EvaluatorMock : Evaluator<double>
        {
            public EvaluatorMock() : base(Definition_Key, Solution_Key)
            {
            }

            public override IEnumerable<double> Evaluate(double definition)
            {
                return new[] { definition };
            }

            public override bool GetLegality(double definition)
            {
                return true;
            }
        }
    }
}