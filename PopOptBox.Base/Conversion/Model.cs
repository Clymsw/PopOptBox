using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Base.Conversion
{
    /// <summary>
    /// The Model manages conversion between optimiser and evaluator
    /// </summary>
    /// <typeparam name="TReality">The type of the object representing reality.</typeparam>
    public abstract class Model<TReality> : IModel
    {
        #region Constructor

        /// <summary>
        /// Constructs the model.
        /// </summary>
        /// <param name="converter">The converter to/from Decision Vector and reality definitions.</param>
        /// <param name="definitionKey">The <see cref="Individual"/> property name for the reality definition.</param>
        protected Model(
            IConverter<TReality> converter,
            string definitionKey)
        {
            conversionModel = converter;
            this.definitionKey = definitionKey;
        }

        #endregion

        #region Fields

        private readonly IConverter<TReality> conversionModel;
        private readonly string definitionKey;

        #endregion

        #region Activity

        /// <summary>
        /// Creates a new suggested Decision Vector based on some real-world logic.
        /// </summary>
        /// <returns>A new Decision Vector.</returns>
        public abstract DecisionVector GetNewDecisionVector();

        /// <summary>
        /// Looks at an <see cref="Individual"/> and inserts any real world information required for evaluation.
        /// </summary>
        /// <param name="ind">The Individual to operate on.</param>
        public void PrepareForEvaluation(Individual ind)
        {
            ind.SetProperty(definitionKey,
                conversionModel.ConvertToReality(ind.DecisionVector));
            ind.SendForEvaluation();
        }

        #endregion
    }
}