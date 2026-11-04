using UnityEngine;
namespace UITK_SimpleTimeline.Examples
{
    /// <summary>
    /// Example curve that affects a transform's position. Uses transform .Find() to parse GameObject hierarchy,
    /// so if you want to get a child of a child use this pattern: childN/subchild#
    /// </summary>
    public class PositionCurve : TypedTimelineCurve<Vector3, string>, ILerpable
    {
        public override string ShorthandCurveName() => "Local Position Curve";
        public override string SpawnKeyframeName() => "Vector3 Keyframe";
        public override string ToAffectName() => "Hierarchy Path to Move";
        private Transform t;
        public override void OnPlay()
        {
            t = root.transform.Find(ToAffect);
        }
        public override Vector3 EvaluateValue(float time)
        {
            Vector3 toReturn;
            
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            toReturn = CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);

            return toReturn;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || !root) return;
            Vector3 toReturn = EvaluateValue(time);
            if (!toReturn.HasNaN()) t.localPosition = toReturn;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Local position curve not yet valid! Give it some keyframes!";
            return $"{ToAffect}.localPos should be: {EvaluateValue(time)}";
        }

        public override string ToString()
        {
            return "Set Root Transform/Local Position";
        }
    }
    /// <summary>
    /// Example curve that affects a transform's position. Uses transform .Find() to parse GameObject hierarchy,
    /// so if you want to get a child of a child use this pattern: childN/subchild#
    /// </summary>
    public class JostlePositionCurve : TypedTimelineCurve<float, string>, ILerpable
    {
        public override string ShorthandCurveName() => "Jostle Position Curve";
        public override string SpawnKeyframeName() => "Float Keyframe";
        public override string ToAffectName() => "Hierarchy Path to Jostle";
        private Vector3 ogLoc;
        private Transform t;
        public override void OnPlay()
        {
            t = root.transform.Find(ToAffect);
            ogLoc = t.localPosition;
        }

        public override float EvaluateValue(float time)
        {
            float rollDistance;
            
            TimelineKeyframe<float>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            rollDistance = CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);

            return rollDistance;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || !root) return;
            t.localPosition = ogLoc + EvaluateValue(time) * CurveMath.RandomSineVector(-180f,180f);
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Jostle position curve not yet valid! Give it some keyframes!";
            return $"{ToAffect}.localPos should be within the distance range: {EvaluateValue(time)}";
        }

        public override string ToString()
        {
            return "Set Root Transform/Jostle Local Position";
        }
    }
    /// <summary>
    /// Example curve that affects a transform's scale. Uses transform .Find() to parse GameObject hierarchy,
    /// so if you want to get a child of a child use this pattern: child1/subchild2
    /// </summary>
    public class ScaleCurve : TypedTimelineCurve<Vector3, string>, ILerpable
    {
        public override string ShorthandCurveName() => "Local Scale Curve";
        public override string SpawnKeyframeName() => "Vector3 Keyframe";
        public override string ToAffectName() => "Hierarchy Path to Scale";
        private Transform t;
        public override void OnPlay()
        {
            t = root.transform.Find(ToAffect);
        }
        public override Vector3 EvaluateValue(float time)
        {
            Vector3 toReturn;

            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            toReturn = CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);
            return toReturn;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || !root) return;
            Vector3 toReturn = EvaluateValue(time);
            if (!toReturn.HasNaN()) t.localScale = toReturn;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Local scale curve not yet valid! Give it some keyframes!";

            return $"{ToAffect}.localScale should be: {EvaluateValue(time)}";
        }

        public override string ToString()
        {
            return "Set Root Transform/Local Scale";
        }
    }
    /// <summary>
    /// Example curve that affects a transform's rotation. Uses transform .Find() to parse GameObject hierarchy,
    /// so if you want to get a child of a child use this pattern: child1/subchild2
    /// </summary>
    public class RotationCurve : TypedTimelineCurve<Vector3, string>, ILerpable
    {
        public override string ShorthandCurveName() => "Local Rotation Curve";
        public override string SpawnKeyframeName() => "Euler Angle (Vec3) Keyframe";
        public override string ToAffectName() => "Hierarchy Path to Rotate";
        private Transform t;
        public override void OnPlay()
        {
            t = root.transform.Find(ToAffect);
        }

        public override Vector3 EvaluateValue(float time)
        {
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);

            Vector3 eulerToBe = CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);
            return eulerToBe;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || !root) return;
            Vector3 eulerToBe = EvaluateValue(time);
            t.localRotation = eulerToBe != Vector3.zero && !eulerToBe.HasNaN() ? Quaternion.Euler(eulerToBe) : Quaternion.identity;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Rotation curve is not yet valid! Give him some keyframes!";
            return $"{ToAffect}.localRot should be: {EvaluateValue(time)}.";
        }

        public override string ToString()
        {
            return "Set Root Transform/Local Rotation";
        }
    }
}
