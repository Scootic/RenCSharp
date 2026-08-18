#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    [UxmlElement]
    public partial class TimelineRuler : VisualElement
    {
        /// <summary>
        /// Basically the length of the ruler. Actually, maybe just always set this bastard to be grown wide as possible...?
        /// </summary>
        private float duration;

        private static Texture2D rulerImage = null;
        private static readonly Color bgColor = new (0.4f, 0.4f, 0.4f, 1);

        private readonly Image display;

        public TimelineRuler() : this(null) { }

        public TimelineRuler(string labelText)
        {
            GrabRulerImage();

            style.left = 0;
            style.right = 0;
            style.height = 50;
            style.backgroundColor = bgColor;
            display = new();
            display.style.left = 0;
            display.style.right = 0;
            display.style.flexGrow = 1;
            display.style.width = style.width;
            display.scaleMode = ScaleMode.StretchToFill;
            display.style.unitySliceTop = 15;
            display.style.height = 50;
            display.image = rulerImage;
            display.style.unitySliceTop = 1;
            Add(display);
            
        }
        public TimelineRuler(string labelText, float dur)
        {
            GrabRulerImage();

            style.left = 0;
            style.right = 0;
            style.height = 50;
            style.backgroundImage = rulerImage;

            DurationChanged(dur);
        }

        private void GrabRulerImage()
        {
            if (rulerImage != null) return;
            rulerImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UITK_Simple_Timeline/UITK_SimpleTimeline_Icons/rulerimage.png");
            if (!rulerImage) Debug.LogError("TimelineRuler.cs couldn't find rulerimage.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
        }

        public void DurationChanged(float t)
        {
            duration = t;
        }
    }
}
#endif