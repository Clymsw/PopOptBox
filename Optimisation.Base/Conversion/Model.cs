using Optimisation.Base.Management;

namespace Optimisation.Base.Conversion
{
    /// <inheritdoc />
    public abstract class Model : IModel
    {
        #region Constructor

        /// <inheritdoc />
        protected Model(
            IConverter converter,
            string keyForEvaluator)
        {
            conversionModel = converter;
            evaluatorDefinitionKey = keyForEvaluator;
        }

        #endregion

        #region Fields

        private readonly IConverter conversionModel;
        private readonly string evaluatorDefinitionKey;

        #endregion

        #region Activity

        /// <inheritdoc />
        public Individual GetNewIndividual()
        {
            return CreateNewIndividual();
        }

        /// <summary>
        /// Creates a new individual using some real-world logic
        /// Called by <see cref="GetNewIndividual" />
        /// </summary>
        /// <returns>A new Individual.</returns>
        protected abstract Individual CreateNewIndividual();

        /// <inheritdoc />
        public void PrepareForEvaluation(Individual ind)
        {
            ind.SetProperty(evaluatorDefinitionKey,
                conversionModel.ConvertToReality(ind.DecisionVector));
            ind.SendForEvaluation();
        }

        #endregion
    }
}