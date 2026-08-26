using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Stupid evil lerper of cubic type. Remember to give your custom types a ToString(), and to add their Assemblies
    /// to the UITK_SimpleTimeline_AssemblyDatabase asset.
    /// </summary>
    /// <typeparam name="T">The type of value being lerped between.</typeparam>
    /// <typeparam name="U">The type of object that is affected by the T value.</typeparam>
    [Serializable]
    public abstract class TypedTimelineCurve<T,U> : TimelineCurve, ILerpable where U : class
    {
        public U ToAffect;
        public WrapMode WrapMode = WrapMode.Clamp;

        public override VisualElement UITKRepresentation(int index)
        {
            //Debug.Log($"CurvesProp Length: {Helper.CurvesProperty.arraySize}.Is property at index {index} null? " + Helper.CurvesProperty.GetArrayElementAtIndex(index));
            return new TimelineCurveField<T,U>("", this, index);
        }

        #region Keyframes
        /// <summary>
        /// PLEASE! PLEASE BY SORTED IN ORDER OF TIME! PLEASE!!!
        /// </summary>
        [SerializeField] protected List<TimelineKeyframe<T>> keyframes = new();
        public List<TimelineKeyframe<T>> Keyframes => keyframes;
        public void AddKeyframeToCurve(float time)
        {
            TimelineKeyframe<T> toAdd = new();
            toAdd.Time = time;

            for (int i = 0; i < Length; i++)
            {
                if (time < keyframes[i].Time)
                {
                    //whenever inserting a keyframe, make it take the value of the previous one?
                    if (i != 0) toAdd.Value = keyframes[i - 1].Value;
                    keyframes.Insert(i, toAdd); return;
                }
            }
            //if we're adding a new keyframe to the "end" of a curve, make it have the same value as the previous last one.
            if(Length > 0) toAdd.Value = keyframes[Length - 1].Value;
            keyframes.Add(toAdd);
            Debug.Log("New keyframes length: " + Length);
        }

        public void RemoveKeyframeFromCurve(float time)
        {
            for (int i = 0; i < Length; i++)
            {
                if (keyframes[i].Time == time) keyframes.RemoveAt(i);
            }
        }

        public override void CleanOutKeyframesAfterTime(float time)
        {
            keyframes.Sort();
            for (int i = Length - 1; i >= 0; i--)
            {
                if (keyframes[i].Time <= time) break;
                keyframes.RemoveAt(i);
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
        public override void SortKeyframes()
        {
            keyframes.Sort();
        }

        protected float[] KeyframeTimes
        {
            get
            {
                float[] allTimes = new float[keyframes.Count];
                for (int i = 0; i < allTimes.Length; i++)
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
            if (!ValidCurve) return null;
            int[] toReturn = new int[2];
            int index = Array.BinarySearch(KeyframeTimes, time);
            if (index < 0)
            {
                index = ~index;
                //index should now be the one higher than time?
                toReturn[1] = index;
                toReturn[0] = (index > 0) ? index - 1 : Length - 1;
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
            if (!ValidCurve) return null;
            TimelineKeyframe<T>[] toReturn = new TimelineKeyframe<T>[2];
            int[] indexes = ClosestTwoIndexes(time);
            toReturn[0] = keyframes[indexes[0]];
            toReturn[1] = keyframes[indexes[1]];
            return toReturn;
        }
        //not sure what I need this for?
        protected T[] ClosestLerpableValues(float time)
        {
            if (!ValidCurve) return null;
            T[] toReturn = new T[2];
            TimelineKeyframe<T>[] temp = ClosestTwoKeyframes(time);
            toReturn[0] = temp[0].Value;
            toReturn[1] = temp[1].Value;
            return toReturn;
        }

        /// <summary>
        /// Is the keyframe count greater than 2? You can't lerp between less-than-equal-to 1 value(s)! Also is there
        /// actually something ToAffect?
        /// </summary>
        protected bool ValidCurve => keyframes.Count >= 2 && ToAffect != null;
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
    
    [Serializable]
    public abstract class TimelineCurve : ILerpable
    {
        protected GameObject root;

        /// <summary>
        /// Basically the same thing as AnimationCurve.Evaluate, 'cept it doesn't return a float. When involved
        /// during a SimpleTimeline.Await, should apply changes to the U ToAffect.
        /// </summary>
        /// <param name="time">The time, in seconds, on the curve that's being grabbed.</param>
        public abstract void Evaluate(float time);
        /// <summary>
        /// A log message that shows what's happening at param:time.
        /// </summary>
        /// <param name="time">The time, in seconds, on the curve that's being grabbed.</param>
        /// <returns>A contextual message to display what sort of things are happening at param:time.</returns>
        public abstract string EvaluateMessage(float time);
        /// <summary>
        /// String used to label what kind of curve is being deleted by right-click menu. Short-hand version of regular ToString().
        /// Probably.
        /// </summary>
        /// <returns></returns>
        public abstract string DeleteCurveName();
        /// <summary>
        /// String used to describe the T value (the value that is stored in each keyframe).
        /// </summary>
        /// <returns></returns>
        public abstract string SpawnKeyframeName();
        /// <summary>
        /// String used to label the TimelineCurve when rendered in SimpleTimelineUITKField
        /// </summary>
        /// <returns></returns>
        public abstract string ToAffectName();

        public abstract void SortKeyframes();

        public abstract void CleanOutKeyframesAfterTime(float t);

        public abstract VisualElement UITKRepresentation(int index);
        /// <summary>
        /// Should only really be used by in-scene components, like SimpleTimelineAnimationComponent.cs
        /// Sets an in-scene GameObject to be the root element to be animated.
        /// </summary>
        /// <param name="go">GameObject (in-scene!) to set as root.</param>
        public void SetRootObject(GameObject go) { root = go; }

        public WrapMode PreWrapMode = WrapMode.Default;
        public WrapMode PostWrapMode = WrapMode.Default;
    }

}
