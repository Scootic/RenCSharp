using UnityEngine;

namespace UITK_SimpleTimeline
{
    public struct CurveMath
    {
        /// <summary>
        /// Returns the in-tangent and out-tangent values of two keyframes.
        /// </summary>
        /// <param name="frames">Keyframe Array of size 2</param>
        /// <returns>The tangents. OutTangent = i0, InTangent = i1</returns>
        public static float[] GetTangents(TimelineKeyframe[] frames)
        {
            float[] toReturn = new float[2];
            float difT = frames[1].Time - frames[0].Time;
            toReturn[0] = frames[0].OutTangent * difT;
            toReturn[1] = frames[1].InTangent * difT;
            return toReturn;
        }

        /// <summary>
        /// The math that provides a way to intepret between float values based on given time and tangents.
        /// </summary>
        /// <param name="startingValues"></param>
        /// <param name="endingValues"></param>
        /// <param name="time"></param>
        /// <param name="startingTangent"></param>
        /// <param name="endingTangent"></param>
        /// <returns>An array of equal size to startingValues, containing the in-betweens.</returns>
        public static float[] CubicHermiteSpline(float[] startingValues, float[] endingValues, float time, float startingTangent, float endingTangent)
        {
            float timeSqr = time * time;
            float timeCub = timeSqr * time;

            float[] toReturn = new float[startingValues.Length];

            for (int i = 0; i < startingValues.Length; i++)
            {
                toReturn[i] = ((2 * timeCub - 3 * timeSqr + 1) * startingValues[i]) +
                    ((timeCub - 2 * timeSqr + time) * startingTangent) +
                    ((-2 * timeCub + 3 * timeSqr) * endingValues[i]) +
                    ((timeCub - timeSqr) * endingTangent);
            }

            return toReturn;
        }
        /// <summary>
        /// Unlike the CubicHermiteSpline float[], attempts to get the value of a normalized cubic hermite 
        /// spline (min val is always 0, max val is always 1).
        /// </summary>
        /// <param name="time">The time (in percent) you want to check.</param>
        /// <param name="startingTangent">The left keyframe's out-tangent.</param>
        /// <param name="endingTangent">The right keyframe's in-tangent.</param>
        /// <returns>A Cubic Hermite Spline between values 0 (bottom left) and 1 (top right).</returns>
        public static float NormalizedCubicHermiteSpline(float time, float startingTangent, float endingTangent)
        {
            float timeSqr = time * time;
            float timeCub = timeSqr * time;

            float toReturn;

            toReturn = ((2 * timeCub - 3 * timeSqr + 1) * 0) + //val1
               ((timeCub - 2 * timeSqr + time) * startingTangent) +
               ((-2 * timeCub + 3 * timeSqr) * 1) + //val2
               ((timeCub - timeSqr) * endingTangent);

            return toReturn;
        }
    }
}
