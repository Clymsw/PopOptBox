using System;
using System.Collections.Generic;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Base.Helpers
{
    public class FitnessCalculatorSingleObjective : IFitnessCalculator
    {
        private readonly Func<double[], double> solutionToFitness;
        private readonly Func<DecisionVector, double> penalty;
        
        /// <summary>
        /// Constructs a calculator for single objective fitness.
        /// This does not depend on the performance of the other <see cref="Individual"/>s in the <see cref="Population"/>.
        /// </summary>
        /// <param name="solutionToFitness">Conversion function to change Solution Vector into Fitness.</param>
        /// <param name="penalty">Function determining what penalty to assign for illegal individuals.</param>
        public FitnessCalculatorSingleObjective(
            Func<double[], double> solutionToFitness,
            Func<DecisionVector, double> penalty)
        {
            this.solutionToFitness = solutionToFitness;
            this.penalty = penalty;
        }

        /// <summary>
        /// Performs the calculation of Solution Vector to Fitness.
        /// If illegal, assigns a penalty, based on the <see cref="DecisionVector"/>
        /// </summary>
        /// <param name="individuals">The individuals to calculate for.</param>
        /// <returns>The Fitness</returns>
        /// <exception cref="InvalidOperationException">Thrown when the individual is not yet evaluated.</exception>
        public void CalculateAndAssignFitness(IEnumerable<Individual> individuals)
        {
            foreach (var individual in individuals)
            {
                if (individual.State != IndividualState.Evaluated)
                    throw new InvalidOperationException("Individual is not evaluated!");

                var fitness = calculateFitness(individual);

                individual.SetFitness(fitness);
            }
        }

        private double calculateFitness(Individual individual)
        {
            //If the individual has been evaluated and is legal, 
            // assign fitness and store in population.
            //If the individual has been evaluated but is not legal, 
            // assign soft penalty and store in population.
            return individual.Legal
                ? solutionToFitness(individual.SolutionVector)
                : penalty(individual.DecisionVector);
        }
    }
}
