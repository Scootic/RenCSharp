using System;
using UnityEngine;
using TangentMode = UnityEditor.AnimationUtility.TangentMode;
namespace UITK_SimpleTimeline
{
    public struct TimelineKeyframe<T> : IComparable
    {
        /// <summary>
        /// In seconds.
        /// </summary>
        public float Time;
        public T Value;
        public float InSlope;
        public float OutSlope;
        public TangentMode TangentMode;
        public WeightedMode WeightedMode;
        public float InWeight;
        public float OutWeight;
        public float InTangent;
        public float OutTangent;

        public readonly int CompareTo(object obj) //make sure we're ordering our lists by time, because duh
        {
            if (obj == null) return 1;
            TimelineKeyframe<T> other = (TimelineKeyframe<T>) obj;
            return Time.CompareTo(other.Time);
        }
    }

    public abstract class TimelineCurve<T>
    {
        /// <summary>
        /// PLEASE! PLEASE BY SORTED IN ORDER OF TIME! PLEASE!!!
        /// </summary>
        public TimelineKeyframe<T>[] Keyframes;
        protected float[] KeyframeTimes
        {
            get
            {
                float[] allTimes = new float[Keyframes.Length];
                for(int i = 0; i < allTimes.Length; i++)
                {
                    allTimes[i] = Keyframes[i].Time;
                }
                return allTimes;
            }
        }
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
                toReturn[0] = (index > 0) ? index - 1 : Keyframes.Length-1;
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
                    toReturn[0] = (index > 0) ? index - 1 : Keyframes.Length - 1;
                }
            }
            return toReturn;
        }
        protected TimelineKeyframe<T>[] ClosestTwoKeyframes(float time)
        {
            TimelineKeyframe<T>[] toReturn = new TimelineKeyframe<T>[2];
            int[] indexes = ClosestTwoIndexes(time);
            toReturn[0] = Keyframes[indexes[0]];
            toReturn[1] = Keyframes[indexes[1]];
            return toReturn;
        }
        protected T[] ClosestLerpableValues(float time)
        {
            T[] toReturn = new T[2];
            TimelineKeyframe<T>[] temp = ClosestTwoKeyframes(time);
            toReturn[0] = temp[0].Value;
            toReturn[1] = temp[1].Value;
            return toReturn;
        }
        public int Length => Keyframes.Length;
        public TimelineKeyframe<T> AtIndex(int i) { return Keyframes[i]; }
        public WrapMode PreWrapMode = WrapMode.Default;
        public WrapMode PostWrapMode = WrapMode.Default;

        public abstract T Evaluate(float time);
    }
}
