using System.Collections.Generic;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Variables;

namespace PopOptBox.Base.Test.Helpers
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

            public override DecisionVector GetNewDecisionVector()
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