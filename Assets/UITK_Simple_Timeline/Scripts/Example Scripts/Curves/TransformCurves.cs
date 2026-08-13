using UnityEngine;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Example curve that affects a transform's position.
    /// </summary>
    public class PositionCurve : TimelineCurve<Vector3, Transform>
    {
        public override Vector3 Evaluate(float time)
        {
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
            ToAffect.localPosition = toReturn;

            return toReturn;
        }
    }
    /// <summary>
    /// Example curve that affects a transform's scale.
    /// </summary>
    public class ScaleCurve : TimelineCurve<Vector3, Transform>
    {
        public override Vector3 Evaluate(float time)
        {
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = GetTangents(toEval);
            float[] cubics = GetCubicValues(time);
            //???????
            float x = cubics[0] * toEval[0].Value.x + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.x;
            float y = cubics[0] * toEval[0].Value.y + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.y;
            float z = cubics[0] * toEval[0].Value.z + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.z;

            Vector3 toReturn = new(x, y, z);
            ToAffect.localScale = toReturn;

            return toReturn;
        }
    }
    /// <summary>
    /// Example curve that affects a transform's rotation.
    /// </summary>
    public class RotationCurve : TimelineCurve<Quaternion, Transform>
    {
        public override Quaternion Evaluate(float time)
        {
            TimelineKeyframe<Quaternion>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = GetTangents(toEval);
            float[] cubics = GetCubicValues(time);

            float x = cubics[0] * toEval[0].Value.x + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.x;
            float y = cubics[0] * toEval[0].Value.y + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.y;
            float z = cubics[0] * toEval[0].Value.z + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.z;
            float w = cubics[0] * toEval[0].Value.w + cubics[1] * tangents[0] + cubics[2] * tangents[1] + cubics[3] * toEval[1].Value.w;

            Quaternion toReturn = new(x,y,z,w);
            ToAffect.localRotation = toReturn;
            return toReturn;
        }
    }
}
