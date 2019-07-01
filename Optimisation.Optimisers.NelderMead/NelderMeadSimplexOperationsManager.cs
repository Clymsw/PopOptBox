using Optimisation.Base.Variables;
using Optimisation.Optimisers.NelderMead.Simplices;
using System;
using System.Collections.Generic;

namespace Optimisation.Optimisers.NelderMead
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
        /// Constructor for the simplex operations manager.
        /// </summary>
        /// <param name="reflectionCoefficient">Coefficient for <see cref="ReflectExpandContract"/> simplex operator, resulting in a <see cref="NelderMeadSimplexOperations.R"/> action.</param>
        /// <param name="expansionCoefficient">Coefficient for <see cref="ReflectExpandContract"/> simplex operator, resulting in a <see cref="NelderMeadSimplexOperations.E"/> action.</param>
        /// <param name="contractionCoefficient">Coefficient for <see cref="ReflectExpandContract"/> simplex operator, resulting in a <see cref="NelderMeadSimplexOperations.C"/> or <see cref="NelderMeadSimplexOperations.K"/> action.</param>
        /// <param name="shrinkageCoefficient">Coefficient for <see cref="Shrink"/> simplex operator, resulting in a <see cref="NelderMeadSimplexOperations.S"/> action.</param>
        public NelderMeadSimplexOperationsManager(double reflectionCoefficient, double expansionCoefficient,
                double contractionCoefficient, double shrinkageCoefficient)
        {
            ParseCoefficientsAndBuild(
                reflectionCoefficient,
                expansionCoefficient,
                contractionCoefficient,
                shrinkageCoefficient);
        }

        public DecisionVector PerformOperation(IEnumerable<DecisionVector> currentSimplex, NelderMeadSimplexOperations operation)
        {
            var newVertex = DecisionVector.CreateFromArray(DecisionSpace.CreateForUniformDoubleArray(0, 0, 0), new double[0]);
            switch (operation)
            {
                case (NelderMeadSimplexOperations.R):
                    newVertex = reflect.Operate(currentSimplex);
                    break;
                case (NelderMeadSimplexOperations.E):
                    newVertex = expand.Operate(currentSimplex);
                    break;
                case (NelderMeadSimplexOperations.C):
                    newVertex = contractOut.Operate(currentSimplex);
                    break;
                case (NelderMeadSimplexOperations.K):
                    newVertex = contractIn.Operate(currentSimplex);
                    break;
                case (NelderMeadSimplexOperations.S):
                    newVertex = shrink.Operate(currentSimplex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, "This operation is not understood.");
            }
            return newVertex;
        }

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
