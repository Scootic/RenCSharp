using System;
using UnityEngine;

namespace UITK_SimpleTimeline
{
    public class PositionCurve : TimelineCurve<Vector3>
    {
        public override Vector3 Evaluate(float time)
        {
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float difT = toEval[1].Time - toEval[0].Time;
            float outTan = toEval[0].OutTangent * difT;
            float inTan = toEval[1].InTangent * difT;

            float timeSqr = time * time;
            float timeCub = time * time * time;

            float a = 2 * timeCub - 3 * timeSqr + 1;
            float b = timeCub - 2 * timeSqr + time;
            float c = time - timeSqr;
            float d = -2 * timeCub + 3 * timeSqr;
            //???????
            return a * toEval[0].Value + b * outTan * Vector3.one + c * inTan * Vector3.one + d * toEval[1].Value;
        }
    }
}
