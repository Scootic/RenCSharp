using RenCSharp.Actors;
using UITK_SimpleTimeline;
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class ActorLocalPositionCurve : TypedTimelineCurve<Vector3, Actor>
    {
        public override string ShorthandCurveName() => "Actor Local Position Curve";
        public override string SpawnKeyframeName() => "Vector3 Keyframe";
        public override string ToAffectName() => "Actor to Move";

        public override void OnPlay()
        {
            if (Object_Factory.TryGetObject(ToAffect.name, out GameObject go)) 
            {
                SetRootObject(go);
            }
            else
            {
                Debug.LogWarning($"Cannot animate Actor: {ToAffect.name}, because it's not present in the scene.");
            }
        }

        private Vector3 EvaluateV3(float time)
        {
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            return CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            root.transform.localPosition = EvaluateV3(time);
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Actor Local Position Curve is not valid.";
            return $"Actor: {ToAffect.name}'s local position at {time}: {EvaluateV3(time)}";
        }

        public override string ToString()
        {
            return "Actor/Local Position Curve";
        }
    }
    public class ActorLocalScaleCurve : TypedTimelineCurve<Vector3, Actor>
    {
        public override string ShorthandCurveName() => "Actor Local Scale Curve";
        public override string SpawnKeyframeName() => "Vector3 Keyframe";
        public override string ToAffectName() => "Actor to Scale";

        public override void OnPlay()
        {
            if (Object_Factory.TryGetObject(ToAffect.name, out GameObject go)) 
            {
                SetRootObject(go);
            }
            else
            {
                Debug.LogWarning($"Cannot animate Actor: {ToAffect.name}, because it's not present in the scene.");
            }
        }

        private Vector3 EvaluateV3(float time)
        {
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            return CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            root.transform.localScale = EvaluateV3(time);
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Actor Local Scale Curve is not valid.";
            return $"Actor: {ToAffect.name}'s local scale at {time}: {EvaluateV3(time)}";
        }

        public override string ToString()
        {
            return "Actor/Local Scale Curve";
        }
    }
    public class ActorLocalRotationCurve : TypedTimelineCurve<Vector3, Actor>
    {
        public override string ShorthandCurveName() => "Actor Local Rotation Curve";
        public override string SpawnKeyframeName() => "Euler Angle (Vec3) Keyframe";
        public override string ToAffectName() => "Actor to Rotate";

        public override void OnPlay()
        {
            if (Object_Factory.TryGetObject(ToAffect.name, out GameObject go)) 
            {
                SetRootObject(go);
            }
            else
            {
                Debug.LogWarning($"Cannot animate Actor: {ToAffect.name}, because it's not present in the scene.");
            }
        }

        private Vector3 EvaluateV3(float time)
        {
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            return CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            root.transform.rotation = Quaternion.Euler(EvaluateV3(time));
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Actor Local Rotation Curve is not valid.";
            return $"Actor: {ToAffect.name}'s local rotation at {time}: {EvaluateV3(time)}";
        }

        public override string ToString()
        {
            return "Actor/Local Rotation Curve";
        }
    }
}
