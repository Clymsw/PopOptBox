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
            get { return reflectionCoefficient; }
            set
            {
                parseCoefficientsAndBuild(
                    value,
                    expansionCoefficient,
                    contractionCoefficient,
                    shrinkageCoefficient);
            }
        }

        private double expansionCoefficient;
        public double ExpansionCoefficient
        {
            get { return expansionCoefficient; }
            set
            {
                parseCoefficientsAndBuild(
                    reflectionCoefficient,
                    value,
                    contractionCoefficient,
                    shrinkageCoefficient);
            }
        }

        private double contractionCoefficient;
        public double ContractionCoefficient
        {
            get { return contractionCoefficient; }
            set
            {
                parseCoefficientsAndBuild(
                    reflectionCoefficient,
                    expansionCoefficient,
                    value,
                    shrinkageCoefficient);
            }
        }

        private double shrinkageCoefficient;
        public double ShrinkageCoefficient
        {
            get { return shrinkageCoefficient; }
            set
            {
                parseCoefficientsAndBuild(
                    reflectionCoefficient,
                    expansionCoefficient,
                    contractionCoefficient,
                    value);
            }
        }
        #endregion

        public override string ToString()
        {
            return "coefficients: " +
                String.Join(", ",
                $"reflection {ReflectionCoefficient}",
                $"expansion {ExpansionCoefficient}",
                $"contraction {ContractionCoefficient}",
                $"shrinkage {ShrinkageCoefficient}");
        }

        #region Simplex Operators
        protected ReflectExpandContract reflect;
        protected ReflectExpandContract expand;
        protected ReflectExpandContract contractOut;
        protected ReflectExpandContract contractIn;
        protected Shrink shrink;
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
            parseCoefficientsAndBuild(
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
            }
            return newVertex;
        }

        private void parseCoefficientsAndBuild(double r, double e, double c, double s)
        {
            if (r > 0)
                reflectionCoefficient = r;
            else
                throw new ArgumentOutOfRangeException("reflectionCoefficient",
                    "Reflection Coefficient must be greater than 0.");

            if (e > 1)
            {
                if (e > r)
                    expansionCoefficient = e;
                else
                    throw new ArgumentOutOfRangeException("expansionCoefficient",
                        "Expansion Coefficient must be greater than Reflection Coefficient.");
            }
            else
                throw new ArgumentOutOfRangeException("expansionCoefficient",
                    "Expansion Coefficient must be greater than 1.");

            if (c > 0 & c < 1)
                contractionCoefficient = c;
            else
                throw new ArgumentOutOfRangeException("contractionCoefficient",
                    "Contraction Coefficient must be between 0 and 1.");

            if (s > 0 & s < 1)
                shrinkageCoefficient = s;
            else
                throw new ArgumentOutOfRangeException("shrinkageCoefficient",
                    "Shrinkage Coefficient must be between 0 and 1.");

            buildOperators();
        }

        private void buildOperators()
        {
            reflect = new ReflectExpandContract(reflectionCoefficient);
            expand = new ReflectExpandContract(reflectionCoefficient * expansionCoefficient);
            contractOut = new ReflectExpandContract(reflectionCoefficient * contractionCoefficient);
            contractIn = new ReflectExpandContract(-contractionCoefficient);
            shrink = new Shrink(shrinkageCoefficient);
        }

    }
}
