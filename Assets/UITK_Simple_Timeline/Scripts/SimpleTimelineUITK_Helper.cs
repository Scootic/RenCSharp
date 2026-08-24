#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Helper class that... helps.
    /// </summary>
    public static class SimpleTimelineUITK_Helper 
    {
        public static Action<SerializedProperty> ReceiveKeyframe;
        public static Action<TimelineCurve> RemoveTimelineCurve;

        public static readonly float PixelWidthPerSeconds = 32f; //?
        public static readonly Color SecondLayerBG = new(0.2f, 0.2f, 0.2f, 1);
        public static readonly Color SecondLayerBorder = new(0.1f, 0.1f, 0.1f, 1);
        public static readonly Color HalfTransparentWhite = new(1, 1, 1, 0.5f);
        public static readonly Color QuarterTransparentWhite = new(1, 1, 1, 0.25f);

        public static readonly string EditorIconAssetPath = "Assets/UITK_Simple_Timeline/UITK_SimpleTimeline_EditorIcons";

        public static readonly StyleLength Auto = new(StyleKeyword.Auto);

        public static Texture2D FullRulerLength = null;

        public static SerializedObject WindowObject;
        public static SerializedProperty SimpleTimelineProperty, CurvesProperty;
    }
}
#endif