using Optimisation.Base.Management;

namespace Optimisation.Base.Conversion
{
    /// <inheritdoc />
    /// <typeparam name="TReality">The type of the object representing reality.</typeparam>
    public abstract class Model<TReality> : IModel
    {
        #region Constructor

        /// <summary>
        /// Constructs the model
        /// </summary>
        /// <param name="converter">The converter to/from Decision Vector and reality definition.</param>
        /// <param name="definitionKey">The <see cref="Individual"/> property key for the reality definition.</param>
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
            ind.SetProperty(definitionKey,
                conversionModel.ConvertToReality(ind.DecisionVector));
            ind.SendForEvaluation();
        }

        #endregion
    }
}