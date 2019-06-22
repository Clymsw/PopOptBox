using Optimisation.Base.Management;

namespace Optimisation.Problems
{
    public interface IProblemSpace
    {
        Individual GetGlobalOptimum();
    }
}
