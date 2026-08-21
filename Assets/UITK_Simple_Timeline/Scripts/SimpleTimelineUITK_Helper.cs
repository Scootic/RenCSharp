#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
namespace UITK_SimpleTimeline
{
    public static class SimpleTimelineUITK_Helper 
    {
        public static Action<SerializedProperty> ReceiveKeyframe;
        public static readonly float PixelWidthPerSeconds = 32f; //?
        public static readonly Color SecondLayerBG = new(0.2f, 0.2f, 0.2f, 1);
        public static readonly Color SecondLayerBorder = new(0.1f, 0.1f, 0.1f, 1);
        public static readonly Color HalfTransparentWhite = new(1, 1, 1, 0.5f);

        public static Texture2D FullRulerLength = null;

        public static SerializedObject WindowObject;
        public static SerializedProperty SimpleTimelineProperty, CurvesProperty;
    }
}
#endif