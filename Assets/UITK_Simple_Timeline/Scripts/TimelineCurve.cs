using System;
using UnityEngine;
using UObject = UnityEngine.Object;
using TangentMode = UnityEditor.AnimationUtility.TangentMode;
using System.Collections.Generic;
namespace UITK_SimpleTimeline
{
    [Serializable]
    public class TimelineKeyframe<T> : UObject, IComparable //?? not sure if declaring this bastard as unityengine.obj is legal
    {
        public T Value;

        /// <summary>
        /// In seconds.
        /// </summary>
        public float Time;
        public float InSlope;
        public float OutSlope;

        public TangentMode TangentMode;
        public WeightedMode WeightedMode;

        public float InWeight;
        public float OutWeight;
        public float InTangent;
        public float OutTangent;

        public int CompareTo(object obj) //super dee duper make sure we're ordering our lists by time, because duh
        {
            if (obj == null) return 1;
            TimelineKeyframe<T> other = (TimelineKeyframe<T>) obj;
            return Time.CompareTo(other.Time);
        }
    }
    /// <summary>
    /// Stupid evil lerper of cubicly type. Remember to give your custom types a ToString(), and to add them
    /// to the Type[] in SimpleTimelineUITKField.cs
    /// </summary>
    /// <typeparam name="T">The type of value being lerped between.</typeparam>
    /// <typeparam name="U">The type of object that is affected.</typeparam>
    [Serializable]
    public abstract class TimelineCurve<T, U> : UObject where U : class
    {
        protected GameObject root;
        public GameObject SetRootObject { set { root = value; } }

        /// <summary>
        /// The object(s?) that will be impacted whenever the curve is evaluated. :)
        /// </summary>
        public U ToAffect;
        /// <summary>
        /// Basically the same thing as AnimationCurve.Evaluate.
        /// </summary>
        /// <param name="time">The time, in seconds, on the curve that's being grabbed.</param>
        /// <returns>The type of value that's in-between the keyframes at time.</returns>
        public abstract T Evaluate(float time);
        /// <summary>
        /// A log message that shows what's happening at param:time.
        /// </summary>
        /// <param name="time">The time, in seconds, on the curve that's being grabbed.</param>
        /// <returns>A contextual message to display what sort of things are happening at param:time.</returns>
        public abstract string EvaluateMessage(float time);

        public WrapMode PreWrapMode = WrapMode.Default;
        public WrapMode PostWrapMode = WrapMode.Default;

        #region Keyframes
        /// <summary>
        /// PLEASE! PLEASE BY SORTED IN ORDER OF TIME! PLEASE!!!
        /// </summary>
        private readonly List<TimelineKeyframe<T>> keyframes = new();
        public List<TimelineKeyframe<T>> Keyframes => keyframes;
        public void AddKeyframeToCurve(float time)
        {
            TimelineKeyframe<T> toAdd = new();

            for(int i = 0; i < Length; i++)
            {
                if (time < keyframes[i].Time) { keyframes.Insert(i, toAdd); return; }
            }
            keyframes.Add(toAdd);
        }

        public void RemoveKeyframeFromCurve(float time)
        {
            for(int i = 0; i < Length; i++)
            {
                if (keyframes[i].Time == time) keyframes.RemoveAt(i);
            }
        }
        public void RemoveKeyframeFromCurve(int index)
        {
            keyframes.RemoveAt(index);
        }
        public void RemoveKeyframeFromCurve(TimelineKeyframe<T> toRemove)
        {
            if (keyframes.Contains(toRemove)) keyframes.Remove(toRemove);
        }

        protected float[] KeyframeTimes
        {
            get
            {
                float[] allTimes = new float[keyframes.Count];
                for(int i = 0; i < allTimes.Length; i++)
                {
                    allTimes[i] = keyframes[i].Time;
                }
                return allTimes;
            }
        }
        public int Length => keyframes.Count;
        public TimelineKeyframe<T> AtIndex(int i) { return keyframes[i]; }
        #endregion

        #region ClosestTwos
        /// <summary>
        /// Try to grab the the two indexes close to a given time using some binarysearch hogwash.
        /// </summary>
        /// <param name="time">Time in seconds.</param>
        /// <returns>An array holding two indexes, the one below/equal to the time, and the one above. For lerping!</returns>
        protected int[] ClosestTwoIndexes(float time)
        {
            int[] toReturn = new int[2];
            int index = Array.BinarySearch(KeyframeTimes, time);
            if(index < 0)
            {
                index = ~index;
                //index should now be the one higher than time?
                toReturn[1] = index;
                toReturn[0] = (index > 0) ? index - 1 : Length-1;
            }
            else
            {
                if (Keyframes[index].Time < time)
                {
                    toReturn[0] = index;
                    toReturn[1] = index + 1;
                }
                else
                {
                    toReturn[1] = index;
                    toReturn[0] = (index > 0) ? index - 1 : Length - 1;
                }
            }
            return toReturn;
        }
        /// <summary>
        /// Get the two closest keyframes around a given time value.
        /// </summary>
        /// <param name="time">Time in seconds.</param>
        /// <returns>An array containing two keyframes: the one before/equal to given time, and the one after.</returns>
        protected TimelineKeyframe<T>[] ClosestTwoKeyframes(float time)
        {
            TimelineKeyframe<T>[] toReturn = new TimelineKeyframe<T>[2];
            int[] indexes = ClosestTwoIndexes(time);
            toReturn[0] = keyframes[indexes[0]];
            toReturn[1] = keyframes[indexes[1]];
            return toReturn;
        }
        //not sure what I need this for?
        protected T[] ClosestLerpableValues(float time)
        {
            T[] toReturn = new T[2];
            TimelineKeyframe<T>[] temp = ClosestTwoKeyframes(time);
            toReturn[0] = temp[0].Value;
            toReturn[1] = temp[1].Value;
            return toReturn;
        }
        #endregion

        #region StupidMath
        /// <summary>
        /// returns the stinkin' expected cubic values for a curve.
        /// </summary>
        /// <param name="time">guh</param>
        /// <returns>an array of length 4! A = i0, B = i1, C = i2, D = i3</returns>
        public virtual float[] GetCubicValues(float time)
        {
            float[] result = new float[4];

            float timeSqr = time * time;
            float timeCub = timeSqr * time;

            result[0] = 2 * timeCub - 3 * timeSqr + 1;
            result[1] = timeCub - 2 * timeSqr + time;
            result[2] = time - timeSqr;
            result[3] = -2 * timeCub + 3 * timeSqr;

            return result;
        }
        /// <summary>
        /// Returns the in-tangent and out-tangent values of two keyframes.
        /// </summary>
        /// <param name="frames">Keyframe Array of size 2</param>
        /// <returns>The tangents. OutTangent = i0, InTangent = i1</returns>
        public virtual float[] GetTangents(TimelineKeyframe<T>[] frames)
        {
            float[] toReturn = new float[2];
            float difT = frames[1].Time - frames[0].Time;
            toReturn[0] = frames[0].OutTangent * difT;
            toReturn[1] = frames[1].InTangent * difT;
            return toReturn;
        }
        #endregion
    }
}
