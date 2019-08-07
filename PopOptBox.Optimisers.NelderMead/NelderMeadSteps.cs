namespace PopOptBox.Optimisers.NelderMead
{
    /// <summary>
    /// Processes possible when creating a new simplex:
    /// rR - try reflect, choose reflect
    /// reR - try reflect, try expand, choose reflect
    /// reE - try reflect, try expand, choose expand
    /// rcC - try reflect, try contract outside, choose contract outside
    /// rcsS - try reflect, try contract outside, try shrink, choose shrink
    /// rkK - try reflect, try contract inside, choose contract inside
    /// rksS - try reflect, try contract inside, try shrink, choose shrink
    /// </summary>
    public enum NelderMeadSteps
    {
        rR = 1,
        reR = 2,
        reE = 3,
        rcC = 4,
        rcsS = 5,
        rkK = 6,
        rksS = 7
    }
}
