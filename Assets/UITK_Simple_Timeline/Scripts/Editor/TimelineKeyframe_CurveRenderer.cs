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
    }
}
#endif