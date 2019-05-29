using System;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Core.Population
{
    /// <summary>
    /// "Individual" class, which contains information about 
    /// a given single solution for an optimisation problem.
    /// </summary>
    /// <typeparam name="TDecVec">The type of the Decision Vector, 
    /// e.g. int for a Genetic Algorithm</typeparam>
    public class Individual<TDecVec>
    {
        #region Fields

        // Definition of the individual from the optimiser
        public readonly IReadOnlyList<TDecVec> DecisionVector;

        // Properties contain any numeric information an 
        // evaluator wishes to add.
        // Not readonly otherwise not cloneable...
        private Dictionary<string, object> properties =
            new Dictionary<string, object>();

        /// <summary>
        /// The solution(s) currently relevant for the optimisation
        /// </summary>
        public double[] SolutionVector { get; private set; }

        /// <summary>
        /// Score for multi-objective optimisation with domination
        /// </summary>
        public double[] Score { get; private set; }

        /// <summary>
        /// The fitness, used to rank individuals and ultimately determine optimality.
        /// Lower is better.
        /// </summary>
        public double Fitness { get; private set; }

        /// <summary>
        /// If the individual is legal or not
        /// </summary>
        public bool Legal { get; private set; }

        /// <summary>
        /// Current state of the individual
        /// </summary>
        public IndividualStates State = IndividualStates.New;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct individual
        /// </summary>
        /// <param name="decisionVector">Vector of type defined at class construction</param>
        public Individual(IEnumerable<TDecVec> decisionVector)
        {
            DecisionVector = decisionVector.ToArray();
        }

        /// <summary>
        /// Returns a shallow copy of the Individual
        /// </summary>
        public Individual<TDecVec> Clone()
        {
            return new Individual<TDecVec>(DecisionVector)
            {
                SolutionVector = SolutionVector,
                Score = Score,
                Fitness = Fitness,
                properties = properties,
                Legal = Legal,
                State = State
            };
        }

        #endregion

        #region Utility

        /// <summary>
        /// Returns the DV as a string
        /// </summary>
        /// <returns>string version of DV</returns>
        public override string ToString()
        {
            return $"{Fitness} [{string.Join(", ", DecisionVector)}]";
        }

        public IEnumerable<string> GetPropertyNames()
        {
            var keynames = properties.Keys;
            return keynames;
        }

        #endregion

        #region Runtime

        /// <summary>
        /// Call when sending for evaluation
        /// </summary>
        /// <exception cref="InvalidOperationException">Individual is not new</exception>
        internal void SendForEvaluation()
        {
            if (State == IndividualStates.New)
                State = IndividualStates.Evaluating;
            else
                throw new InvalidOperationException("Individual is not new!");
        }

        /// <summary>
        /// Call when finished evaluating
        /// </summary>
        /// <exception cref="InvalidOperationException">Individual is not evaluating</exception>
        internal void FinishEvaluating()
        {
            if (State == IndividualStates.Evaluating)
                State = IndividualStates.Evaluated;
            else
                throw new InvalidOperationException("Individual is not evaluating!");
        }

        /// <summary>
        /// Stores a Key-Value pair in the individual's properties.
        /// If key already exists, value is over-written with a warning.
        /// </summary>
        /// <param name="key">String acting as key for the property</param>
        /// <param name="value">Value of the property</param>
        public void SetProperty(string key, object value)
        {
            if (properties.ContainsKey(key))
            {
                // TODO: Use logger
                Console.WriteLine("Warning: Over-writing property '" + key + "'");
                var oldValue = properties[key];
                Console.WriteLine("Old value: " + oldValue);
                properties[key] = value;
            }
            else
            {
                properties.Add(key, value);
            }
        }

        /// <summary>
        /// Retrieves the value for a given property key, if it exists.
        /// Otherwise returns a zero element double array and a warning.
        /// </summary>
        /// <param name="key">String acting as key for the property</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when key is not present.</exception>
        /// <returns>Property value</returns>
        public T GetProperty<T>(string key)
        {
            if (!properties.TryGetValue(key, out var value))
                throw new ArgumentOutOfRangeException(nameof(key), key, "Key not found");
            return (T) value;
        }

        /// <summary>
        /// Assigns SolutionVector based on a given Property key value
        /// </summary>
        /// <param name="keyname">Name of property key</param>
        public void SetSolution(string keyname)
        {
            var solutionValue = GetProperty<double[]>(keyname);
            SolutionVector = solutionValue ??
                             throw new ArgumentOutOfRangeException(nameof(keyname), "Invalid key to set solution!");
        }

        /// <summary>
        /// Assigns Score based on a function which must be passed
        /// in as a delegate that converts a double array into a single value
        /// </summary>
        /// <param name="solToScore">Delegate converting solution to score</param>
        public void SetScore(Func<double[], double[]> solToScore)
        {
            var scoreValue = solToScore(SolutionVector);
            Score = scoreValue;
        }

        /// <summary>
        /// Assigns Fitness based on a function which must be passed
        /// in as a delegate that converts a double array into a single value
        /// </summary>
        /// <param name="scoreToFit">Delegate converting (solution or) score to fitness</param>
        public void SetFitness(Func<double[], double> scoreToFit)
        {
            var fitnessValue = Score == null
                ? scoreToFit(SolutionVector)
                : scoreToFit(Score);
            Fitness = fitnessValue;
        }

        #endregion

        #region Equals, GetHashCode

        /// <summary>
        /// We use a very limited form of Equals (should really be ApproxEquals)
        /// which only checks for Decision Vector equivalence.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Individual<TDecVec> other))
                return false;

            return DecisionVector.Equals(other.DecisionVector);
        }

        public override int GetHashCode()
        {
            return new
            {
                DecisionVector
            }.GetHashCode();
        }

        #endregion
    }
}