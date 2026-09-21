using System;
using UnityEngine;
using UnityEngine.UIElements;
using TangentMode = UITK_SimpleTimeline.TimelineKeyframeTangentMode;
namespace UITK_SimpleTimeline
{
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
