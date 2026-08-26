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
        /// <summary>
        /// Get a current keyframe whenever you click on a TimelineKnob in the Editor Window
        /// </summary>
        public static Action<SerializedProperty, VisualElement> ReceiveKeyframe;
        /// <summary>
        /// Fires when you right-click delete a TimelineCurve from the Editor Window
        /// </summary>
        public static Action<TimelineCurve> RemoveTimelineCurve;

        /// <summary>
        /// The amount of pixels that equals one second along the TimelineRuler
        /// </summary>
        public static readonly float PixelWidthPerSeconds = 32f; //?
        public static float MaxPixelWidth;
        public static readonly Color SecondLayerBG = new(0.2f, 0.2f, 0.2f, 1);
        public static readonly Color SecondLayerBorder = new(0.1f, 0.1f, 0.1f, 1);
        public static readonly Color SelectedKeyframe = new(0.7f, 0.95f, 0, 1);
        public static readonly Color HalfTransparentWhite = new(1, 1, 1, 0.5f);
        public static readonly Color QuarterTransparentBlack = new(0, 0, 0, 0.25f);
        public static readonly Color QuarterTransparentWhite = new(1, 1, 1, 0.25f);

        /// <summary>
        /// The asset path to load all of the editor icon assets. Change this if you, for some reason, move
        /// the EditorIcons folder from outside of the UITK_Simple_Timeline folder, or otherwise nest it.
        /// <br/><br/>
        /// Should include the Assets folder as the root of the format: Assets/.../UITK_Simple_Timeline/EditorIcons
        /// </summary>
        public static readonly string EditorIconAssetPath = "Assets/UITK_Simple_Timeline/EditorIcons";

        public static readonly StyleLength Auto = new(StyleKeyword.Auto);
        /// <summary>
        /// Texture used by the TimelineScrollView's content to be a slightly grayed out underlay to extend the ruler's
        /// measurements down without being too obtrusive. (Hopefully.)
        /// </summary>
        public static Texture2D FullRulerLength = null;

        public static SerializedObject WindowObject;
        public static SerializedProperty SimpleTimelineProperty, CurvesProperty;
        /// <summary>
        /// Do every single function that's relevant to saving and updating all of the SerializedProperties so that your
        /// data is actually saved after you're done with the Editor Window.
        /// </summary>
        public static void ApplyChangesToObject()
        {
            WindowObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(WindowObject.targetObject);
            WindowObject.Update();
        }
    }
}
#endif