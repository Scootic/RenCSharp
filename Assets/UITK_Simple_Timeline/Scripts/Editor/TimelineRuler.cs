#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

        public TimelineRuler() : this(null) { }

        public TimelineRuler(string labelText)
        {
            GrabRulerImage();
        }
        public TimelineRuler(string labelText, float dur)
        {
            GrabRulerImage();

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