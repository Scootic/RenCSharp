#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    public class TimelineKeyframe_CurveRenderer : VisualElement, IRegeneratableElement
    {
        private const int lineResolution = 50;
        TimelineKeyframe left, right;

        public TimelineKeyframe_CurveRenderer()
        {
            
        }

        public void ReceiveKeyframes(TimelineKeyframe newL, TimelineKeyframe newR)
        {
            left = newL;
            right = newR;
            RegenerateElement();
        }

        public void RegenerateElement()
        {

        }

        public float SplineAtTime(float time, float startingTangent, float endingTangent)
        {
            float timeSqr = time * time;
            float timeCub = timeSqr * time;

            float toReturn;

            toReturn = ((2 * timeCub - 3 * timeSqr + 1) * 0) + //val1
               ((timeCub - 2 * timeSqr + time) * startingTangent) +
               ((-2 * timeCub + 3 * timeSqr) * 1) + //val2
               ((timeCub - timeSqr) * endingTangent);

            return toReturn;
        }
    }
}
#endif