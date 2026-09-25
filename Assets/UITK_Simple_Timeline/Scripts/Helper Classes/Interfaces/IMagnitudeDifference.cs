namespace UITK_SimpleTimeline
{
    public interface IMagnitudeDifference<T>
    {
        /// <summary>
        /// Compare a float representation of the "magnitude" of two different items. 
        /// Used for scaling tangents to affect even the largest of keyframes.
        /// </summary>
        /// <param name="leftItem">Lefthand item.</param>
        /// <param name="rightItem">Righthand item.</param>
        /// <returns>The user set float magnitude between the two items.</returns>
        public float Difference(T leftItem, T rightItem);
    }
}
