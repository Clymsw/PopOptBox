namespace PopOptBox.Optimisers.StructuredSearch
{
    /// <summary>
    /// Enumerator for all 5 possible simplex operations:
    /// R: reflect
    /// E: expand
    /// C: Contract outside
    /// K: Contract inside
    /// S: Shrink
    /// </summary>
    public enum NelderMeadSimplexOperations
    {
        R = 1,
        E = 2,
        C = 3,
        K = 4,
        S = 5
    }
}
