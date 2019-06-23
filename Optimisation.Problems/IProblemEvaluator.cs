using Optimisation.Base.Variables;

namespace Optimisation.Problems
{
    public interface IProblemEvaluator
    {
        /// <summary>
        /// Returns the information about the location of the global optimum.
        /// </summary>
        /// <returns></returns>
        DecisionVector GetGlobalOptimum();
    }
}
