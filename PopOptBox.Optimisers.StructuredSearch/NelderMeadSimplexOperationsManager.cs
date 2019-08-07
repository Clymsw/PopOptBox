using System;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.StructuredSearch.Simplices;

namespace PopOptBox.Optimisers.StructuredSearch
{
    /// <summary>
    /// Ensures all simplex operations have consistent coefficients,
    /// and provides easy way of accessing them.
    /// </summary>
    public class NelderMeadSimplexOperationsManager
    {
        #region Coefficients
        private double reflectionCoefficient;
        public double ReflectionCoefficient
        {
            get => reflectionCoefficient;
            set =>
                ParseCoefficientsAndBuild(
                    value,
                    expansionCoefficient,
                    contractionCoefficient,
                    shrinkageCoefficient);
        }

        private double expansionCoefficient;
        public double ExpansionCoefficient
        {
            get => expansionCoefficient;
            set =>
                ParseCoefficientsAndBuild(
                    reflectionCoefficient,
                    value,
                    contractionCoefficient,
                    shrinkageCoefficient);
        }

        private double contractionCoefficient;
        public double ContractionCoefficient
        {
            get => contractionCoefficient;
            set =>
                ParseCoefficientsAndBuild(
                    reflectionCoefficient,
                    expansionCoefficient,
                    value,
                    shrinkageCoefficient);
        }

        private double shrinkageCoefficient;
        public double ShrinkageCoefficient
        {
            get => shrinkageCoefficient;
            set =>
                ParseCoefficientsAndBuild(
                    reflectionCoefficient,
                    expansionCoefficient,
                    contractionCoefficient,
                    value);
        }
        #endregion

        public override string ToString()
        {
            return "coefficients: " +
                string.Join(", ",
                $"reflection {ReflectionCoefficient}",
                $"expansion {ExpansionCoefficient}",
                $"contraction {ContractionCoefficient}",
                $"shrinkage {ShrinkageCoefficient}");
        }

        #region Simplex Operators

        private ReflectExpandContract reflect;
        private ReflectExpandContract expand;
        private ReflectExpandContract contractOut;
        private ReflectExpandContract contractIn;
        private Shrink shrink;
        #endregion

        /// <summary>
        ///     Constructor for the simplex operations manager.
        /// </summary>
        /// <param name="coefficients">
        ///     Instance of the <see cref="HyperParameterManager"/> with coefficients for
        ///     the <see cref="ReflectExpandContract"/> and <see cref="Shrink"/> simplex operators.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coefficients are not meaningful.</exception>
        public NelderMeadSimplexOperationsManager(HyperParameterManager coefficients)
        {
            ParseCoefficientsAndBuild(
                coefficients.GetHyperParameterValue<double>(NelderMeadHyperParameters.Reflection_Coefficient),
                coefficients.GetHyperParameterValue<double>(NelderMeadHyperParameters.Expansion_Coefficient),
                coefficients.GetHyperParameterValue<double>(NelderMeadHyperParameters.Contraction_Coefficient),
                coefficients.GetHyperParameterValue<double>(NelderMeadHyperParameters.Shrinkage_Coefficient));
        }

        /// <summary>
        /// Performs a particular simplex manipulation function
        /// </summary>
        /// <param name="currentSimplex">The simplex to operate on.</param>
        /// <param name="operation">A choice of operation from <see cref="NelderMeadSimplexOperations"/>.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="operation"/> is not known.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the new location is not valid.</exception>
        public DecisionVector PerformOperation(Simplex currentSimplex, NelderMeadSimplexOperations operation)
        {
            switch (operation)
            {
                case (NelderMeadSimplexOperations.R):
                    return reflect.Operate(currentSimplex);
                case (NelderMeadSimplexOperations.E):
                    return expand.Operate(currentSimplex);
                case (NelderMeadSimplexOperations.C):
                    return contractOut.Operate(currentSimplex);
                case (NelderMeadSimplexOperations.K):
                    return contractIn.Operate(currentSimplex);
                case (NelderMeadSimplexOperations.S):
                    return shrink.Operate(currentSimplex);
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, "This operation is not understood.");
            }
        }

        /// <summary>
        /// Ensures all coefficients are present and correct. 
        /// </summary>
        /// <remarks>Many of the basic checks are performed by the <see cref="DecisionSpace"/>s in <see cref="NelderMeadHyperParameters"/>.</remarks>
        private void ParseCoefficientsAndBuild(
            double reflectCoefficient, 
            double expandCoefficient, 
            double contractCoefficient, 
            double shrinkCoefficient)
        {
            if (reflectCoefficient > 0)
                reflectionCoefficient = reflectCoefficient;
            else
                throw new ArgumentOutOfRangeException(nameof(reflectCoefficient),
                    "Reflection Coefficient must be greater than 0.");

            if (expandCoefficient > 1)
            {
                if (expandCoefficient > reflectCoefficient)
                    expansionCoefficient = expandCoefficient;
                else
                    throw new ArgumentOutOfRangeException(nameof(expandCoefficient),
                        "Expansion Coefficient must be greater than Reflection Coefficient.");
            }
            else
                throw new ArgumentOutOfRangeException(nameof(expandCoefficient),
                    "Expansion Coefficient must be greater than 1.");

            if (contractCoefficient > 0 & contractCoefficient < 1)
                contractionCoefficient = contractCoefficient;
            else
                throw new ArgumentOutOfRangeException(nameof(contractCoefficient),
                    "Contraction Coefficient must be between 0 and 1.");

            if (shrinkCoefficient > 0 & shrinkCoefficient < 1)
                shrinkageCoefficient = shrinkCoefficient;
            else
                throw new ArgumentOutOfRangeException(nameof(shrinkCoefficient),
                    "Shrinkage Coefficient must be between 0 and 1.");

            BuildOperators();
        }

        private void BuildOperators()
        {
            reflect = new ReflectExpandContract(reflectionCoefficient);
            expand = new ReflectExpandContract(reflectionCoefficient * expansionCoefficient);
            contractOut = new ReflectExpandContract(reflectionCoefficient * contractionCoefficient);
            contractIn = new ReflectExpandContract(-contractionCoefficient);
            shrink = new Shrink(shrinkageCoefficient);
        }

    }
}
