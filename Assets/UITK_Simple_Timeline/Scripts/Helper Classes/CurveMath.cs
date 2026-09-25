using System;
using UnityEngine;
using TangentMode = UITK_SimpleTimeline.TimelineKeyframeTangentMode;
namespace UITK_SimpleTimeline
{
    public static class CurveMath
    {
        /// <summary>
        /// Returns the in-tangent and out-tangent values of two keyframes, multiplied by the magnitude difference between two keyframes.
        /// <br/><br/>IMagnitudeDifference interface is for custom classes that you want to be able to compare the magnitudes of. If the T
        /// type is not a valid type (float, vector2, vector3, or IMagnitude), the tangents will by default be scaled by 1 (which is to say, not).
        /// </summary>
        /// <param name="frames">Keyframe Array of size 2</param>
        /// <returns>The tangents. OutTangent = i0, InTangent = i1</returns>
        public static float[] GetTangents<T>(TimelineKeyframe<T>[] frames)
        {
            float[] toReturn = new float[2];
            float dif = 1;
            switch (frames[0].Value) //scale the tangents by the difference in magnitudes.
            {
                case float f:
                    if (frames[1].Value is float f2) dif = f2 - f;
                    break;
                case Vector3 v3:
                    if (frames[1].Value is Vector3 v3_2) dif = (v3_2 - v3).magnitude;
                    break;
                case Vector2 v2:
                    if (frames[1].Value is Vector2 v2_2) dif = (v2_2 - v2).magnitude;
                    break;
                case IMagnitudeDifference<T> imd:
                    dif = imd.Difference(frames[0].Value, frames[1].Value);
                    break;
                default:
                    dif = 1;
                    break;
            }
            toReturn[0] = frames[0].OutTangent * dif;
            toReturn[1] = frames[1].InTangent * dif;
            return toReturn;
        }
        /// <summary>
        /// Untyped version of GetTangents() using only abstract Keyframes. Returns the in-tangent and out-tangent, without multiplying tangent strength.
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static float[] GetTangents(TimelineKeyframe[] frames)
        {
            float[] toReturn = new float[2];
            toReturn[0] = frames[0].OutTangent;
            toReturn[1] = frames[1].InTangent;
            return toReturn;
        }

        #region CubicHermiteSplines
        /// <summary>
        /// The math that provides a way to intepret between float values based on given time and tangents.
        /// </summary>
        /// <param name="startingValues"></param>
        /// <param name="endingValues"></param>
        /// <param name="time">The time (in percent) you want to check.</param>
        /// <param name="startingTangent"></param>
        /// <param name="endingTangent"></param>
        /// <returns>An array of equal size to startingValues, containing the in-betweens.</returns>
        public static float[] CubicHermiteSpline(float[] startingValues, float[] endingValues, float time, float startingTangent, float endingTangent, TangentMode tangentMode = TangentMode.Free)
        {
            float[] toReturn = new float[startingValues.Length];

            for (int i = 0; i < startingValues.Length; i++)
            {
                toReturn[i] = CubicHermiteSpline(startingValues[i], endingValues[i], time, startingTangent, endingTangent, tangentMode);
            }

            return toReturn;
        }

        /// <summary>
        /// The math that provides a way to intepret between float values based on given time and tangents.
        /// </summary>
        /// <param name="startingValues"></param>
        /// <param name="endingValues"></param>
        /// <param name="time">The time (in percent) you want to check.</param>
        /// <param name="startingTangent"></param>
        /// <param name="endingTangent"></param>
        /// <returns>An array of equal size to startingValues, containing the in-betweens.</returns>
        public static Vector3 CubicHermiteSpline(Vector3 startingValues, Vector3 endingValues, float time, float startingTangent, float endingTangent, TangentMode tangentMode = TangentMode.Free)
        {
            return CubicHermiteSpline(startingValues.ToArray(), endingValues.ToArray(), time, startingTangent, endingTangent, tangentMode).ToVector3();
        }
        /// <summary>
        /// Unlike the CubicHermiteSpline float[], attempts to get the value of a normalized cubic hermite 
        /// spline (min val is always 0, max val is always 1).
        /// </summary>
        /// <param name="time">The time (in percent) you want to check.</param>
        /// <param name="startingTangent">The left keyframe's out-tangent.</param>
        /// <param name="endingTangent">The right keyframe's in-tangent.</param>
        /// <returns>A Cubic Hermite Spline between values 0 (bottom left) and 1 (top right).</returns>
        public static float NormalizedCubicHermiteSpline(float time, float startingTangent, float endingTangent, TangentMode tangentMode = TangentMode.Free)
        {
            return CubicHermiteSpline(0,1,time,startingTangent,endingTangent,tangentMode);
        }
        /// <summary>
        /// A single cubic hermite spline, only comparing and returning one value.
        /// </summary>
        /// <param name="leftValue">Initial value.</param>
        /// <param name="rightValue">Ending value.</param>
        /// <param name="time">The time (in percent) being checked.</param>
        /// <param name="startingTangent">The left out-tangent.</param>
        /// <param name="endingTangent">The right in-tangent.</param>
        /// <param name="tangentMode">Determines how tangents affect the curve.</param>
        /// <returns></returns>
        public static float CubicHermiteSpline(float leftValue, float rightValue, float time, float startingTangent, float endingTangent, TangentMode tangentMode, bool debug = false) 
        {
            float toReturn;
            float timeSqr = time * time;
            float timeCub = timeSqr * time;
            switch (tangentMode)
            {
                case TangentMode.Free:
                    toReturn = ((2 * timeCub - 3 * timeSqr + 1) * leftValue) + //val1
                    ((timeCub - 2 * timeSqr + time) * startingTangent) + //startTan
                    ((-2 * timeCub + 3 * timeSqr) * rightValue) + //val2
                    ((timeCub - timeSqr) * endingTangent); //endTan
                    if (debug) Debug.Log($"Val1 {(2 * timeCub - 3 * timeSqr + 1) * leftValue} + " +
                        $"startTan {(timeCub - 2 * timeSqr + time) * startingTangent} " +
                        $"+ Val2 {(-2 * timeCub + 3 * timeSqr) * rightValue} " +
                        $"+ endTan {(timeCub - timeSqr) * endingTangent} = {toReturn}");
                    return toReturn;

                case TangentMode.Auto:
                    toReturn = ((2 * timeCub - 3 * timeSqr + 1) * leftValue) + //val1
                    ((-2 * timeCub + 3 * timeSqr) * rightValue); //val2
                    return toReturn;

                case TangentMode.Linear:
                    toReturn = Mathf.Lerp(leftValue, rightValue, time);
                    return toReturn;

                case TangentMode.ClampedFree:
                    toReturn = ((2 * timeCub - 3 * timeSqr + 1) * leftValue) + //val1
                    ((timeCub - 2 * timeSqr + time) * startingTangent) + //startTan
                    ((-2 * timeCub + 3 * timeSqr) * rightValue) + //val2
                    ((timeCub - timeSqr) * endingTangent); //endTan
                    float min = Mathf.Min(leftValue, rightValue);
                    float max = min == leftValue ? rightValue : leftValue;
                    toReturn = Mathf.Clamp(toReturn, min, max);
                    return toReturn;

                case TangentMode.Cliff: //could probably just say return leftVal at all times...
                    toReturn = time < 0.99f ? leftValue : rightValue;
                    return toReturn;

                default:
                    return 0;
            }
        }
        #endregion

        #region Async
        /// <summary>
        /// Background thread version of single cubic hermite spline, only comparing and returning one value.
        /// </summary>
        /// <param name="leftValue">Initial value.</param>
        /// <param name="rightValue">Ending value.</param>
        /// <param name="time">The time (in percent) being checked.</param>
        /// <param name="startingTangent">The left out-tangent.</param>
        /// <param name="endingTangent">The right in-tangent.</param>
        /// <param name="tangentMode">Determines how tangents affect the curve.</param>
        /// <returns></returns>
        public static async Awaitable<float> CubicHermiteSplineAsync(float leftValue, float rightValue, float time, float startingTangent, float endingTangent, TangentMode tangentMode)
        {
            await Awaitable.BackgroundThreadAsync();
            float toReturn;
            float timeSqr = time * time;
            float timeCub = timeSqr * time;
            switch (tangentMode)
            {
                case TangentMode.Free:
                    toReturn = ((2 * timeCub - 3 * timeSqr + 1) * leftValue) + //val1
                    ((timeCub - 2 * timeSqr + time) * startingTangent) + //startTan
                    ((-2 * timeCub + 3 * timeSqr) * rightValue) + //val2
                    ((timeCub - timeSqr) * endingTangent); //endTan
                    return toReturn;

                case TangentMode.Auto:
                    toReturn = ((2 * timeCub - 3 * timeSqr + 1) * leftValue) + //val1
                    ((-2 * timeCub + 3 * timeSqr) * rightValue); //val2
                    return toReturn;

                case TangentMode.Linear:
                    toReturn = Mathf.Lerp(leftValue, rightValue, time);
                    return toReturn;

                case TangentMode.ClampedFree:
                    toReturn = ((2 * timeCub - 3 * timeSqr + 1) * leftValue) + //val1
                    ((timeCub - 2 * timeSqr + time) * startingTangent) + //startTan
                    ((-2 * timeCub + 3 * timeSqr) * rightValue) + //val2
                    ((timeCub - timeSqr) * endingTangent); //endTan
                    float min = Mathf.Min(leftValue, rightValue);
                    float max = min == leftValue ? rightValue : leftValue;
                    toReturn = Mathf.Clamp(toReturn, min, max);
                    return toReturn;

                case TangentMode.Cliff:
                    toReturn = time < 1 ? leftValue : rightValue;
                    return toReturn;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Background thread version of CubicHermiteSpline array. Math should be done on background thread.
        /// </summary>
        /// <param name="startingValues"></param>
        /// <param name="endingValues"></param>
        /// <param name="time">The time (in percent) you want to check.</param>
        /// <param name="startingTangent"></param>
        /// <param name="endingTangent"></param>
        /// <returns>An array of equal size to startingValues, containing the in-betweens.</returns>
        public static async Awaitable<float[]> CubicHermiteSplineAsync(float[] startingValues, float[] endingValues, float time, float startingTangent, float endingTangent, TangentMode tangentMode = TangentMode.Free)
        {
            await Awaitable.BackgroundThreadAsync();
            float[] toReturn = new float[startingValues.Length];

            for (int i = 0; i < startingValues.Length; i++)
            {
                toReturn[i] = await CubicHermiteSplineAsync(startingValues[i], endingValues[i], time, startingTangent, endingTangent, tangentMode);
            }

            return toReturn;
        }

        /// <summary>
        /// Background thread version of CubicHermiteSpline V3
        /// </summary>
        /// <param name="startingValues"></param>
        /// <param name="endingValues"></param>
        /// <param name="time">The time (in percent) you want to check.</param>
        /// <param name="startingTangent"></param>
        /// <param name="endingTangent"></param>
        /// <returns>An array of equal size to startingValues, containing the in-betweens.</returns>
        public static async Awaitable<Vector3> CubicHermiteSplineAsync(Vector3 startingValues, Vector3 endingValues, float time, float startingTangent, float endingTangent, TangentMode tangentMode = TangentMode.Free)
        {
            await Awaitable.BackgroundThreadAsync();
            float[] guh = await CubicHermiteSplineAsync(startingValues.ToArray(), endingValues.ToArray(), time, startingTangent, endingTangent, tangentMode);
            return guh.ToVector3();
        }

        /// <summary>
        /// Background thread version of NormalizedCubicHermiteSpline()
        /// </summary>
        /// <param name="time">The time (in percent) you want to check.</param>
        /// <param name="startingTangent">The left keyframe's out-tangent.</param>
        /// <param name="endingTangent">The right keyframe's in-tangent.</param>
        /// <returns>A Cubic Hermite Spline between values 0 (bottom left) and 1 (top right).</returns>
        public static async Awaitable<float> NormalizedCubicHermiteSplineAsync(float time, float startingTangent, float endingTangent, TangentMode tangentMode = TangentMode.Free)
        {
            await Awaitable.BackgroundThreadAsync();
            return await CubicHermiteSplineAsync(0, 1, time, startingTangent, endingTangent, tangentMode);
        }
        #endregion
    }
}
