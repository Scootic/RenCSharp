using System;

namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Only exists because actual TangentMode is for some reason Editor only. Sad!
    /// </summary>
    [Serializable]
    public enum TimelineKeyframeTangentMode
    {
        /// <summary>
        /// Free to go as the tangents declare the curve to be.
        /// </summary>
        Free = 1,
        /// <summary>
        /// Automatically creates a curve between the two keyframes. S shape?
        /// </summary>
        Auto = 2,
        /// <summary>
        /// Transition between two keyframes linearly.
        /// </summary>
        Linear = 4,
        /// <summary>
        /// Same as free, but values are clamped to never exceed starting or ending value.
        /// </summary>
        ClampedFree = 8,
        /// <summary>
        /// Stays at left keyframe value until touching right keyframe.
        /// </summary>
        Cliff = 16
    }
}
