using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TangentMode = UITK_SimpleTimeline.TimelineKeyframeTangentMode;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Stupid evil lerper of cubic type. Remember to give your custom types a ToString(), and to add their Assemblies
    /// to the UITK_SimpleTimeline_AssemblyDatabase asset. Double typed.
    /// </summary>
    /// <typeparam name="T">The type of value being lerped between. (Keyframe Values)</typeparam>
    /// <typeparam name="U">The type of object that is affected by the T value.</typeparam>
    [Serializable]
    public abstract class TypedTimelineCurve<T,U> : TypedTimelineCurve<T>, ILerpable where U : notnull
    {
        public U ToAffect;
        
#if UNITY_EDITOR
        public override VisualElement UITKRepresentation(int index)
        {
            return new DoubleTypedTimelineCurveField<T,U>("", this, index);
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
                Debug.Log("The forbidden else statement in TimelineCurve.ClosestTwoIndexes() has been called?!?");
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

    [Serializable]
    public abstract class ArrayTimelineCurve<T, U> : ArrayTimelineCurve<T>, ILerpable where T : notnull where U : notnull
    {
        public U ToAffect;
#if UNITY_EDITOR
        public override VisualElement UITKRepresentation(int index)
        {
            return new DoubleTypedArrayTimelineCurveField<T, U>("", this, index);
        }
#endif
        protected override bool ValidCurve
        {
            get
            {
                if (ToAffect == null) return false;
                for (int i = 0; i < keyframes.Length; i++)
                {
                    if (keyframes[i].Count < 2) return false;
                }
                return true;
            }
        }

        public override string ToString()
        {
            return "Generic Array Double Typed Timeline Curve";
        }
    }

    [Serializable]
    public abstract class ArrayTimelineCurve<T> : TimelineCurve, ILerpable where T : notnull
    {
#if UNITY_EDITOR
        public override VisualElement UITKRepresentation(int index)
        {
            return new SingleTypedArrayTimelineCurveField<T>("", this, index);
        }
#endif

        [SerializeField] protected TimelineListWrapper<TimelineKeyframe<T>>[] keyframes = new TimelineListWrapper<TimelineKeyframe<T>>[0];
        public TimelineListWrapper<TimelineKeyframe<T>>[] Keyframes => keyframes;

        public abstract T[] EvaluateValue(float time);

        public override void AddKeyframeToCurve(float t)
        {
            for(int i = 0; i < keyframes.Length; i++)
            {
                AddKeyframeToCurve(t, i);
            }
        }

        public override void AddKeyframeToCurve(float t, int index)
        {
            TimelineKeyframe<T> toAdd = new()
            {
                Time = t,
                DefaultTangentMode = DefaultTangentMode(),
                TangentMode = DefaultTangentMode(),
                ExcludedTangentModes = ExcludedKeyframeTangentModes(),
                ArrayIndex = index
            };

            for (int i = 0; i < keyframes[index].Count; i++)
            {
                if (t < keyframes[index][i].Time)
                {
                    //whenever inserting a keyframe, make it take the value of the one on its left?
                    if (i != 0) toAdd.Value = keyframes[index][i - 1].Value;
                    keyframes[index].Insert(i, toAdd); return;
                }
            }
            //if we're adding a new keyframe to the "end" of a curve, make it have the same value as the previous last one.
            if (keyframes[index].Count > 0) toAdd.Value = keyframes[index][keyframes[index].Count - 1].Value;
            //make the new keyframe its default value if it's an IDefaultableNotNull type
            else
            {
                IDefaultableNotNull<T> idnn = toAdd.Value as IDefaultableNotNull<T>;
                if (idnn != null) toAdd.Value = idnn.Default();
            }
            //otherwise, just add a blank keyframe if it's the first of its kind.
            keyframes[index].Add(toAdd);
        }

        public void RemoveKeyframeFromCurve(float time, int index)
        {
            for (int i = 0; i < keyframes[index].Count; i++)
            {
                if (keyframes[index][i].Time == time) keyframes[index].RemoveAt(i);
            }
        }
        public void RemoveKeyframeFromCurve(int keyframeIndex, int curveIndex)
        {
            keyframes[curveIndex].RemoveAt(keyframeIndex);
        }
        public void RemoveKeyframeFromCurve(TimelineKeyframe<T> toRemove, int index)
        {
            if (keyframes[index].Contains(toRemove)) keyframes[index].Remove(toRemove);
        }

        public override void CleanOutKeyframesAfterTime(float time, int index)
        {
            keyframes[index].Sort();
            for (int i = keyframes[index].Count - 1; i >= 0; i--)
            {
                if (keyframes[index][i].Time <= time) break;
                keyframes[index].RemoveAt(i);
            }
        }

        public override void CleanOutKeyframesAfterTime(float t)
        {
            for(int i = 0; i < keyframes.Length; i++)
            {
                CleanOutKeyframesAfterTime(t, i);
            }
        }

        public override void SortKeyframes()
        {
            foreach (TimelineListWrapper<TimelineKeyframe<T>> curve in keyframes)
            {
                curve.Sort();
            }
        }

        public TimelineKeyframe<T> AtIndex(int curveindex, int keyframeindex) { return keyframes[curveindex][keyframeindex]; }

        public TimelineKeyframe<T> AtTime(float t, int index)
        {
            foreach (TimelineKeyframe<T> keyframe in keyframes[index])
            {
                if (ApproxFloat(keyframe.Time, t)) return keyframe;
            }
            return null;
        }

        public List<TimelineKeyframe<T>> AtTime(float t)
        {
            List<TimelineKeyframe<T>> toReturn = new();
            foreach(TimelineListWrapper<TimelineKeyframe<T>> list in keyframes)
            {
                foreach(TimelineKeyframe<T> keyframe in list)
                {
                    if (ApproxFloat(keyframe.Time, t)) { toReturn.Add(keyframe); break; }
                }
                toReturn.Add(null); //add null if you can go through the whole list.
            }
            return toReturn;
        }

        protected float[][] KeyframeTimes
        {
            get
            {
                float[][] allTimes = new float[keyframes.Length][];
                for (int i = 0; i < allTimes.Length; i++)
                {
                    allTimes[i] = new float[keyframes[i].Count];
                    for (int j = 0; j < allTimes[i].Length; i++)
                    {
                        allTimes[i][j] = keyframes[i][j].Time;
                    }
                }
                return allTimes;
            }
        }

        public override int[][] ArrayClosestTwoIndexes(float t)
        {
            if (!ValidCurve) return null;
            int[][] toReturn = new int[keyframes.Length][];
            for (int i = 0; i < keyframes.Length; i++)
            {
                //works if in-between two keyframes?
                toReturn[i] = new int[2];
                int index = Array.BinarySearch(KeyframeTimes, t);
                if (index < 0)
                {
                    index = ~index;
                    //Debug.Log($"Doing the weird bitwise chicanery because binary search gave a negative index?!? The bitfliped: {index}");
                    //index should now be the one higher than time?
                    toReturn[i][1] = index;
                    if (t > KeyframeTimes[i][keyframes[i].Count - 1]) //if the time has already passed the last key frame
                    {
                        index = keyframes[i].Count - 1;
                        switch (WrappingMode)
                        {
                            case WrapMode.Clamp: //clamp to basically lerp between itself (no motion)
                                toReturn[i][0] = index;
                                toReturn[i][1] = index;
                                break;
                            default: //otherwise loop back around, with the start key frame being the last value???
                                toReturn[i][1] = 0;
                                toReturn[i][0] = index;
                                break;
                        }
                    }
                    else if (t < KeyframeTimes[i][0]) //if the time is before the first key frame
                    {
                        switch (WrappingMode)
                        {
                            case WrapMode.Clamp:
                                toReturn[i][0] = 0;
                                toReturn[i][1] = 0;
                                break;
                            default:
                                toReturn[i][0] = keyframes[i].Count - 1;
                                toReturn[i][1] = 0;
                                break;
                        }
                    }
                    else toReturn[i][0] = (index > 0) ? index - 1 : keyframes[i].Count - 1;
                }
                else//this else is never ever called?!?!
                {
                    Debug.Log("The forbidden else statement in TimelineCurve.ClosestTwoIndexes() has been called?!?");
                    if (Keyframes[i][index].Time < t)
                    {
                        toReturn[i][0] = index;
                        toReturn[i][1] = index + 1;
                    }
                    else
                    {
                        toReturn[i][1] = index;
                        toReturn[i][0] = (index > 0) ? index - 1 : keyframes[i].Count - 1;
                    }
                }
            }
            return toReturn;
        }

        protected TimelineKeyframe<T>[][] ArrayClosestTwoKeyframes(float time)
        {
            if (!ValidCurve) return null;
            TimelineKeyframe<T>[][] toReturn = new TimelineKeyframe<T>[keyframes.Length][];
            int[][] indexes = ArrayClosestTwoIndexes(time);
            for (int i = 0; i < keyframes.Length; i++)
            {
                toReturn[i][0] = keyframes[i][indexes[i][0]];
                toReturn[i][1] = keyframes[i][indexes[i][1]];
            }
            return toReturn;
        }
        /// <summary>
        /// Overrides all stuff to create a new array of appropriate size.
        /// </summary>
        public int SetKeyframesArrayLength { set { keyframes = new TimelineListWrapper<TimelineKeyframe<T>>[value];
                for(int i = 0; i < keyframes.Length; i++)
                {
                    keyframes[i] = new TimelineListWrapper<TimelineKeyframe<T>>(0);
                }
            } }

        public abstract int DefaultArrayLength();

        protected virtual bool ValidCurve 
        {
            get
            {
                for(int i = 0; i < keyframes.Length; i++)
                {
                    if (keyframes[i].Count < 2) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Only exists for ArrayTimelineCurves.
        /// </summary>
        /// <returns>string[0] by default.</returns>
        public abstract string[] SpawnKeyframeNames();

        public override string ToString() 
        {
            return "Generic Array Typed Timeline Curve";
        }
    }

    [Serializable]
    public abstract class TimelineCurve : ILerpable
    {
        /// <summary>
        /// 0 to include all.
        /// <br/><br/>15 to exclude all but Cliff.
        /// </summary>
        /// <returns></returns>
        protected virtual int ExcludedKeyframeTangentModes() => 0;

        protected virtual TangentMode DefaultTangentMode() => TangentMode.Free;

        protected GameObject root;

        [Tooltip("Decides how keyframes lerp whenever the time is before the first keyframe," +
            " or after the last keyframe.\nWrapMode.Clamp keeps things static in those extremes, whereas " +
            "the other types of WrapModes cause keyframes to lerp beyond their border.")]
        public WrapMode WrappingMode = WrapMode.Clamp;

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
        public abstract string ShorthandCurveName();
        /// <summary>
        /// String used to describe the T value (the value that is stored in each keyframe).
        /// </summary>
        /// <returns></returns>
        public abstract string SpawnKeyframeName();
        
        /// <summary>
        /// String used to label the TimelineCurve's U value when rendered in SimpleTimelineUITKField
        /// </summary>
        /// <returns></returns>
        public abstract string ToAffectName();
        /// <summary>
        /// Re-order keyframes based on their time value (smallest time with smallest index, largest time with largest index).
        /// </summary>
        public abstract void SortKeyframes();
        /// <summary>
        /// Get rid of all keyframes whose time value are larger than the given time.
        /// </summary>
        /// <param name="t">The given time in seconds.</param>
        public virtual void CleanOutKeyframesAfterTime(float t) { Debug.LogWarning("Clean out Keyframes After Time not implemented!"); return; }
        /// <summary>
        /// Only exists for ArrayTimelineCurves. Similar behavior to CleanOutKeyframesAfterTime only affecting a certain layer.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="index"></param>
        public virtual void CleanOutKeyframesAfterTime(float t, int index) { return; }
        /// <summary>
        /// Add a new keyframe to the curve at the given time.
        /// </summary>
        /// <param name="t">The given time in seconds.</param>
        public virtual void AddKeyframeToCurve(float t) { Debug.LogWarning("Add Keyframe To Curve not implemented!"); return; }
        /// <summary>
        /// Only exists for ArrayTimelineCurves. Similar behavior to AddKeyframeToCurve, adding a keyframe to a specific layer.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="i"></param>
        public virtual void AddKeyframeToCurve(float t, int i) { return; }

        /// <summary>
        /// Get the closest two indexes at the given time.
        /// </summary>
        /// <param name="t">The given time in seconds.</param>
        /// <returns>Closest two indexes, index0 is to the left, index1 is to the right, unless given time
        /// is larger the last keyframe, or smaller than the first.</returns>
        public virtual int[] ClosestTwoIndexes(float t) { Debug.LogWarning("Closest Two Indexes not implemented!"); return null; }
        public virtual int[][] ArrayClosestTwoIndexes(float t) { return new int[0][]; }
#if UNITY_EDITOR
        public abstract VisualElement UITKRepresentation(int index);
#endif
        /// <summary>
        /// Should only really be used by in-scene components, like SimpleTimelineAnimationComponent.cs
        /// Sets an in-scene GameObject to be the root element to be animated.
        /// </summary>
        /// <param name="go">GameObject (in-scene!) to set as root.</param>
        public void SetRootObject(GameObject go) { root = go; }
        /// <summary>
        /// Called by SimpleTimeline whenever it's played, before any evaluation.
        /// </summary>
        public virtual void OnPlay() { return; }
 
        /// <summary>
        /// Attempts to see if two floats are "close enough," based on a given tolerance
        /// </summary>
        /// <param name="a">A float</param>
        /// <param name="b">Another float</param>
        /// <param name="tolerance">The largest acceptable deviation between float a and b to return true.</param>
        /// <returns>Mathf.Abs(biggest - smallest) lessthan tolerance</returns>
        public bool ApproxFloat(float a, float b, float tolerance = 0.01f)
        {
            float biggest = Mathf.Max(a, b);
            float smallest = Mathf.Min(a, b);

            return Mathf.Abs(biggest - smallest) < tolerance;
        }

        public float TimeToKeyframePercent(float absoluteTime, float keyframe1Time, float keyframe2Time)
        {
            float percent;

            float timeAfterK1 = absoluteTime - keyframe1Time;
            float distance = keyframe2Time - keyframe1Time;
            percent = timeAfterK1 / distance;

            return percent;
        }

        public override string ToString()
        {
            return "Abstract Timeline Curve";
        }
    }
    
}
