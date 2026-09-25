using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Stupid evil lerper of cubic type. Remember to give your custom types a ToString(), and to add their Assemblies
    /// to the UITK_SimpleTimeline_AssemblyDatabase asset. Double typed.
    /// </summary>
    /// <typeparam name="T">The type of value being lerped between. (Keyframe Values)</typeparam>
    /// <typeparam name="U">The type of object that is affected by the T value.</typeparam>
    [Serializable]
    public abstract class TypedTimelineCurve<T, U> : TypedTimelineCurve<T>, ILerpable where U : notnull
    {
        public U ToAffect;

#if UNITY_EDITOR
        public override VisualElement UITKRepresentation(int index)
        {
            return new DoubleTypedTimelineCurveField<T, U>("", this, index);
        }
#endif
        public override string ToString()
        {
            return "Generic Two-Typed Timeline Curve";
        }

        /// <summary>
        /// Is the keyframe count greater than 2? You can't lerp between less-than-equal-to 1 value(s)! Also is there
        /// actually something ToAffect?
        /// </summary>
        protected override bool ValidCurve => keyframes.Count >= 2 && ToAffect != null;
    }
    /// <summary>
    /// Stupid evil lerper of cubic type. Remember to give your custom types a ToString(), and to add their Assemblies
    /// to the UITK_SimpleTimeline_AssemblyDatabase asset. Single typed.
    /// </summary>
    /// <typeparam name="T">The type of value being lerped between. (Keyframe Values)</typeparam>
    [Serializable]
    public abstract class TypedTimelineCurve<T> : TimelineCurve, ILerpable where T : notnull
    {
#if UNITY_EDITOR
        public override VisualElement UITKRepresentation(int index)
        {
            return new SingleTypedTimelineCurveField<T>("", this, index);
        }
        /// <summary>
        /// Only to be used by TimelineCurveFields to preview
        /// </summary>
        [SerializeField] protected T TemporaryRep;
#endif
        public abstract T EvaluateValue(float time);

        #region Keyframes
        /// <summary>
        /// PLEASE! PLEASE BY SORTED IN ORDER OF TIME! PLEASE!!!
        /// </summary>
        [SerializeField] protected List<TimelineKeyframe<T>> keyframes = new();
        public List<TimelineKeyframe<T>> Keyframes => keyframes;
        public override void AddKeyframeToCurve(float time)
        {
            TimelineKeyframe<T> toAdd = new()
            {
                Time = time,
                DefaultTangentMode = DefaultTangentMode(),
                TangentMode = DefaultTangentMode(),
                ExcludedTangentModes = ExcludedKeyframeTangentModes()
            };

            for (int i = 0; i < Length; i++)
            {
                if (time < keyframes[i].Time)
                {
                    //whenever inserting a keyframe, make it take the value of the one on its left?
                    if (i != 0) toAdd.Value = keyframes[i - 1].Value;
                    keyframes.Insert(i, toAdd); return;
                }
            }
            //if we're adding a new keyframe to the "end" of a curve, make it have the same value as the previous last one.
            if (Length > 0) toAdd.Value = keyframes[Length - 1].Value;
            //make the new keyframe its default value if it's an IDefaultableNotNull type
            else
            {
                IDefaultableNotNull<T> idnn = toAdd.Value as IDefaultableNotNull<T>;
                if (idnn != null) toAdd.Value = idnn.Default();
            }
            //otherwise, just add a blank keyframe if it's the first of its kind.
            keyframes.Add(toAdd);
        }

        public void AddKeyframeToCurve(float time, T value)
        {
            TimelineKeyframe<T> toAdd = new()
            {
                Time = time,
                DefaultTangentMode = DefaultTangentMode(),
                TangentMode = DefaultTangentMode(),
                ExcludedTangentModes = ExcludedKeyframeTangentModes(),
                Value = value
            };
            keyframes.Add(toAdd);
        }

        public void SetKeyframeAtTime(float time, T value)
        {
            foreach (TimelineKeyframe<T> keyframe in keyframes)
            {
                if (keyframe.Time == time) { keyframe.Value = value; break; }
            }
        }

        public void RemoveKeyframeFromCurve(float time)
        {
            for (int i = 0; i < Length; i++)
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

        public override void CleanOutKeyframesAfterTime(float time)
        {
            keyframes.Sort();
            for (int i = Length - 1; i >= 0; i--)
            {
                if (keyframes[i].Time <= time) break;
                keyframes.RemoveAt(i);
            }
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

        public TimelineKeyframe<T> AtTime(float t)
        {
            foreach (TimelineKeyframe<T> keyframe in keyframes)
            {
                if (ApproxFloat(keyframe.Time, t)) return keyframe;
            }
            return null;
        }
        #endregion

        #region ClosestTwos
        /// <summary>
        /// Try to grab the the two indexes close to a given time using some binarysearch hogwash.
        /// </summary>
        /// <param name="time">Time in seconds.</param>
        /// <returns>An array holding two indexes, the one below/equal to the time, and the one above. For lerping!</returns>
        public override int[] ClosestTwoIndexes(float time)
        {
            if (!ValidCurve) return null;
            //works if in-between two keyframes?
            int[] toReturn = new int[2];
            int index = Array.BinarySearch(KeyframeTimes, time);
            if (index < 0)
            {
                index = ~index;
                //Debug.Log($"Doing the weird bitwise chicanery because binary search gave a negative index?!? The bitfliped: {index}");
                //index should now be the one higher than time?
                toReturn[1] = index;
                if (time > KeyframeTimes[Length - 1]) //if the time has already passed the last key frame
                {
                    index = Length - 1;
                    switch (WrappingMode)
                    {
                        case WrapMode.Clamp: //clamp to basically lerp between itself (no motion)
                            toReturn[0] = index;
                            toReturn[1] = index;
                            break;
                        default: //otherwise loop back around, with the start key frame being the last value???
                            toReturn[1] = 0;
                            toReturn[0] = index;
                            break;
                    }
                }
                else if (time < KeyframeTimes[0]) //if the time is before the first key frame
                {
                    switch (WrappingMode)
                    {
                        case WrapMode.Clamp:
                            toReturn[0] = 0;
                            toReturn[1] = 0;
                            break;
                        default:
                            toReturn[0] = Length - 1;
                            toReturn[1] = 0;
                            break;
                    }
                }
                else toReturn[0] = (index > 0) ? index - 1 : Length - 1;
            }
            else//this else is never ever called?!?!
            {
                //Debug.Log("The forbidden else statement in TimelineCurve.ClosestTwoIndexes() has been called?!?");
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
        protected virtual bool ValidCurve => keyframes.Count >= 2;
        #endregion

        public override string ToString()
        {
            return "Generic Typed Timeline Curve";
        }
    }
}
