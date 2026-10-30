namespace RenCSharp.EXPERIMENTAL
{
    public static class MiscExtensions
    {
        /// <summary>
        /// Checks if a given float is within a given range of a "center point" on a number line. (No actually number line, just greater-than less-than checking.)
        /// </summary>
        /// <param name="evaluated">The middle position on the number line.</param>
        /// <param name="toEvaluate">The position we want to compare on the number line.</param>
        /// <param name="range">The +- range to check within on a number line (full size both directions, diameter not radius if that makes sense).</param>
        /// <returns>If the given float is within range of the evaluated float.</returns>
        public static bool WithinRange(this float evaluated, float toEvaluate, float range)
        {
            //Imagine a number line:
            //if outside of range in left direction
            if (toEvaluate < evaluated - range) return false;
            //if outside of range in right direction
            if (toEvaluate > evaluated + range) return false;
            return true;
        }
    }
}
