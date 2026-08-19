using UnityEngine;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Example curve that affects a transform's position. Uses transform .Find() to parse GameObject hierarchy,
    /// so if you want to get a child of a child use this pattern: child1/subchild2
    /// </summary>
    public class PositionCurve : TimelineCurve<Vector3, StringWrapper>
    {
        public override Vector3 Evaluate(float time)
        {
            if(root == null) { Debug.LogWarning("No root object for position curve!"); return Vector3.zero; }
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = GetTangents(toEval);
            float[] cubics = GetCubicValues(time);

            //general pattern for evaluating float values from keyframes:
            ///float example = cubics[0] * firstKeyFrameValue + cubics[1] * tangents[0] + cubics[2] * tangents[1]
            ///+ cubics[3] * secondKeyFrameValue
            float x = cubics[0] * toEval[0].Value.x + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.x;
            float y = cubics[0] * toEval[0].Value.y + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.y;
            float z = cubics[0] * toEval[0].Value.z + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.z;

            Vector3 toReturn = new(x, y, z);
            Transform t = root.transform.Find(ToAffect.value);
            t.localPosition = toReturn;

            return toReturn;
        }

        public override string EvaluateMessage(float time)
        {
            return $"{ToAffect}.localPos should be: {Evaluate(time)}";
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
    public class ScaleCurve : TimelineCurve<Vector3, StringWrapper>
    {
        public override Vector3 Evaluate(float time)
        {
            if (root == null) { Debug.LogWarning("No root object for scale curve!"); return Vector3.zero; }
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = GetTangents(toEval);
            float[] cubics = GetCubicValues(time);
            //???????
            float x = cubics[0] * toEval[0].Value.x + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.x;
            float y = cubics[0] * toEval[0].Value.y + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.y;
            float z = cubics[0] * toEval[0].Value.z + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.z;

            Vector3 toReturn = new(x, y, z);
            Transform t = root.transform.Find(ToAffect.value);
            t.localScale = toReturn;

            return toReturn;
        }

        public override string EvaluateMessage(float time)
        {
            return $"{ToAffect}.localScale should be: {Evaluate(time)}";
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
    public class RotationCurve : TimelineCurve<Quaternion, StringWrapper>
    {
        public override Quaternion Evaluate(float time)
        {
            if (root == null) { Debug.LogWarning("No root object for rotation curve!"); return Quaternion.identity; }
            TimelineKeyframe<Quaternion>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = GetTangents(toEval);
            float[] cubics = GetCubicValues(time);

            float x = cubics[0] * toEval[0].Value.x + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.x;
            float y = cubics[0] * toEval[0].Value.y + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.y;
            float z = cubics[0] * toEval[0].Value.z + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.z;
            float w = cubics[0] * toEval[0].Value.w + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.w;

            Quaternion toReturn = new(x,y,z,w);
            Transform t = root.transform.Find(ToAffect.value);
            t.localRotation = toReturn;
            return toReturn;
        }

        public override string EvaluateMessage(float time)
        {
            return $"{ToAffect}.localRot should be: {Evaluate(time)}";
        }

        public override string ToString()
        {
            return "Set Root Transform/Local Rotation";
        }
    }
}
