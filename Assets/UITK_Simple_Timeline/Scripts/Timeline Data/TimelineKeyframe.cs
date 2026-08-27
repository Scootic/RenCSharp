
using System;
using UnityEngine;
#if UNITY_EDITOR
using static UnityEditor.AnimationUtility;
#endif
namespace UITK_SimpleTimeline
{
    [Serializable]
    public class TimelineKeyframe<T> : IComparable
    {
        public T Value;
        /// <summary>
        /// In seconds.
        /// </summary>
        [Min(0)]public float Time;
        public float InSlope;
        public float OutSlope;
#if UNITY_EDITOR
        public TangentMode TangentMode;
#endif
        public WeightedMode WeightedMode;

        public float InWeight;
        public float OutWeight;
        public float InTangent;
        public float OutTangent;

        public int CompareTo(object obj) //super dee duper make sure we're ordering our lists by time, because duh
        {
            if (obj == null) return 1;
            TimelineKeyframe<T> other = (TimelineKeyframe<T>)obj;
            return Time.CompareTo(other.Time);
        }
#if UNITY_EDITOR
        public static Keyframe GetKeyframeFromTK(TimelineKeyframe<T> tk)
        {
            //doesn't set value at all?
            Keyframe toReturn = new();

            toReturn.time = tk.Time;
            toReturn.weightedMode = tk.WeightedMode;
            toReturn.tangentMode = (int)tk.TangentMode;
            toReturn.inWeight = tk.InWeight;
            toReturn.outWeight = tk.OutWeight;
            toReturn.inTangent = tk.InTangent;
            toReturn.outTangent = tk.OutTangent;

            return toReturn;
        }
#endif
    }
}
