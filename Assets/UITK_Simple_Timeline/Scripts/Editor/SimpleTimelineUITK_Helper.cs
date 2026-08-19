using System;
using UnityEngine;
namespace UITK_SimpleTimeline.Editor
{
    public static class SimpleTimelineUITK_Helper 
    {
        public static Action<TimelineKeyframe<object>> ReceiveKeyframe;
        public static readonly double PixelWidthPerSeconds = 23.6220472441;
        public static readonly Color SecondLayerBG = new(0.2f, 0.2f, 0.2f, 1);
        public static readonly Color SecondLayerBorder = new(0.1f, 0.1f, 0.1f, 1);
        public static readonly Color HalfTransparentWhite = new(1, 1, 1, 0.5f);
    }
}
