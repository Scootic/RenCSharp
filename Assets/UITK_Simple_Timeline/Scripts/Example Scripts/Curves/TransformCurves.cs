using UnityEngine;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Example curve that affects a transform's position. Uses transform .Find() to parse GameObject hierarchy,
    /// so if you want to get a child of a child use this pattern: child1/subchild2
    /// </summary>
    public class PositionCurve : TypedTimelineCurve<Vector3, string>, ILerpable
    {
        public override string ShorthandCurveName() => "Local Position Curve";
        public override string SpawnKeyframeName() => "Vector3 Keyframe";
        public override string ToAffectName() => "Hierarchy Path to Move";
        private Vector3 EvaluateV3(float time)
        {
            Vector3 toReturn = new();
            
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = GetTangents(toEval);
            float[] cubics = GetCubicValues(time);

            //general pattern for evaluating float values from keyframes:
            ///float example = cubics[0] * firstKeyFrameValue + cubics[1] * tangents[0] + cubics[2] * tangents[1]
            ///+ cubics[3] * secondKeyFrameValue
            float x = cubics[0] * toEval[0].Value.x + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.x;
            float y = cubics[0] * toEval[0].Value.y + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.y;
            float z = cubics[0] * toEval[0].Value.z + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.z;

            toReturn = new(x, y, z);
            if (root == null) { return toReturn; }
            Transform t = root.transform.Find(ToAffect);
            t.localPosition = toReturn;
            return toReturn;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            EvaluateV3(time);
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Local position curve not yet valid! Give it some keyframes!";
            return $"{ToAffect}.localPos should be: {EvaluateV3(time)}";
        }

        public override string ToString()
        {
            return "Set Root Transform/Local Position";
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
        private Vector3 EvaluateV3(float time)
        {
            Vector3 toReturn = new();

            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = GetTangents(toEval);
            float[] cubics = GetCubicValues(time);
            //???????
            float x = cubics[0] * toEval[0].Value.x + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.x;
            float y = cubics[0] * toEval[0].Value.y + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.y;
            float z = cubics[0] * toEval[0].Value.z + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.z;
            if (root == null) {return toReturn; }
            toReturn = new(x, y, z);
            Transform t = root.transform.Find(ToAffect);
            t.localScale = toReturn;
            return toReturn;
        }

        public override void Evaluate(float t)
        {
            if (!ValidCurve) return;
            EvaluateV3(t);
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Local scale curve not yet valid! Give it some keyframes!";

            return $"{ToAffect}.localScale should be: {EvaluateV3(time)}";
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
    public class RotationCurve : TypedTimelineCurve<Quaternion, string>, ILerpable
    {
        public override string ShorthandCurveName() => "Local Rotation Curve";
        public override string SpawnKeyframeName() => "Quaternion Keyframe";
        public override string ToAffectName() => "Hierarchy Path to Rotate";
        private Quaternion EvaluateQ(float time)
        {
            Quaternion toReturn = new();

            TimelineKeyframe<Quaternion>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = GetTangents(toEval);
            float[] cubics = GetCubicValues(time);

            float x = cubics[0] * toEval[0].Value.x + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.x;
            float y = cubics[0] * toEval[0].Value.y + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.y;
            float z = cubics[0] * toEval[0].Value.z + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.z;
            float w = cubics[0] * toEval[0].Value.w + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.w;

            toReturn = new(x,y,z,w);
            if (root == null) { return toReturn; }
            return toReturn;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            EvaluateQ(time);
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Rotation curve is not yet valid! Give him some keyframes!";

            return $"{ToAffect}.localRot should be: {EvaluateQ(time)}";
        }

        public override string ToString()
        {
            return "Set Root Transform/Local Rotation";
        }
    }
}
