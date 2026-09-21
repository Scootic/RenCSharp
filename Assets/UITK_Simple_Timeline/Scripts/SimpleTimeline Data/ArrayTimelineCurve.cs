using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TangentMode = UITK_SimpleTimeline.TimelineKeyframeTangentMode;
namespace UITK_SimpleTimeline
{
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

        /// <summary>
        /// Only to be used by TimelineCurveFields to preview
        /// </summary>
        [SerializeField] protected T[] TemporaryRep;
#endif

        [SerializeField] protected TimelineListWrapper<TimelineKeyframe<T>>[] keyframes = new TimelineListWrapper<TimelineKeyframe<T>>[0];
        public TimelineListWrapper<TimelineKeyframe<T>>[] Keyframes => keyframes;

        public abstract T[] EvaluateValue(float time);

        public override void AddKeyframeToCurve(float t)
        {
            for (int i = 0; i < keyframes.Length; i++)
            {
                AddKeyframeToCurve(t, i);
            }
        }

        public void AddKeyframeToCurve(float t, T[] values)
        {
            for (int i = 0; i < keyframes.Length; i++)
            {
                AddKeyframeToCurve(t, i, values[i]);
            }
        }

        public void SetKeyframeAtTime(float time, int index, T value)
        {
            foreach (TimelineKeyframe<T> keyframe in keyframes[index])
            {
                if (keyframe.Time == time) { keyframe.Value = value; break; }
            }
        }

        public void AddKeyframeToCurve(float t, int index, T value)
        {
            TimelineKeyframe<T> toAdd = new()
            {
                Time = t,
                DefaultTangentMode = DefaultTangentMode(),
                TangentMode = DefaultTangentMode(),
                ExcludedTangentModes = ExcludedKeyframeTangentModes(),
                ArrayIndex = index,
                Value = value
            };
            keyframes[index].Add(toAdd);
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
            for (int i = 0; i < keyframes.Length; i++)
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
            foreach (TimelineListWrapper<TimelineKeyframe<T>> list in keyframes)
            {
                foreach (TimelineKeyframe<T> keyframe in list)
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
                    for (int j = 0; j < allTimes[i].Length; j++)
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
                int index = Array.BinarySearch(KeyframeTimes[i], t);
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
                else
                {
                    //Debug.Log("The forbidden else statement in TimelineCurve.ClosestTwoIndexes() has been called?!?");
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
                toReturn[i] = new TimelineKeyframe<T>[2];
                toReturn[i][0] = keyframes[i][indexes[i][0]];
                toReturn[i][1] = keyframes[i][indexes[i][1]];
            }
            return toReturn;
        }
        /// <summary>
        /// Overrides all stuff to create a new array of appropriate size.
        /// </summary>
        public int SetKeyframesArrayLength
        {
            set
            {
                keyframes = new TimelineListWrapper<TimelineKeyframe<T>>[value];
                for (int i = 0; i < keyframes.Length; i++)
                {
                    keyframes[i] = new TimelineListWrapper<TimelineKeyframe<T>>(0);
                }
            }
        }

        public abstract int DefaultArrayLength();

        protected virtual bool ValidCurve
        {
            get
            {
                for (int i = 0; i < keyframes.Length; i++)
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
}
