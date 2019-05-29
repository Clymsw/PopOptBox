using Optimisation.Core.Population;

namespace Optimisation.Core.Model
{
    /// <inheritdoc />
    public abstract class Model<TDecVec, TReality> : IModel<TDecVec>
    {
        #region Constructor

        /// <inheritdoc />
        protected Model(
            IConverter<TDecVec, TReality> converter,
            string keyForEvaluator)
        {
            conversionModel = converter;
            evaluatorDefinitionKey = keyForEvaluator;
        }

        #endregion

        #region Fields

        private readonly IConverter<TDecVec, TReality> conversionModel;
        private readonly string evaluatorDefinitionKey;

        #endregion

        #region Activity

        /// <inheritdoc />
        public Individual<TDecVec> GetNewIndividual()
        {
            return CreateNewIndividual();
        }

        /// <summary>
        ///     Creates a new individual using some real-world logic
        ///     Called by <see cref="GetNewIndividual" />
        /// </summary>
        /// <returns>A new Individual.</returns>
        protected abstract Individual<TDecVec> CreateNewIndividual();

        /// <inheritdoc />
        public void PrepareForEvaluation(Individual<TDecVec> ind)
        {
            ind.SetProperty(evaluatorDefinitionKey,
                conversionModel.ConvertToReality(ind.DecisionVector));
            ind.SendForEvaluation();
        }

        #endregion
    }
}