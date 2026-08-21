#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK_SimpleTimeline
{
    [UxmlElement]
    public partial class TimelineKnob<T> : BaseField<TimelineKeyframe<T>>
    {
        protected TimelineKnob<T> clipboard = null;
        protected static Texture2D knobImage = null;
        public Action DeleteKnobAction;
        public SerializedProperty KnobProperty;
        public GenericMenu DeleteMe
        {
            get
            {
                GenericMenu toReturn = new();

                toReturn.AddItem(new GUIContent($"Delete Knob at {value.Time}."), false, delegate
                {
                    DeleteKnobAction.Invoke();
                });

                toReturn.AddSeparator("Clipboard");
                toReturn.AddItem(new GUIContent($"Copy Knob at {value.Time}s."), false, delegate { clipboard = this; });
                if (clipboard == null) toReturn.AddDisabledItem(new GUIContent($"No knob to paste."));
                else toReturn.AddItem(new GUIContent($"Paste Knob from {value.Time}s."), false, delegate
                {
                    float oldTime = value.Time;
                    TimelineKeyframe<T> toSet = clipboard.value;
                    toSet.Time = oldTime;

                    value = toSet;
                });

                return toReturn;
            }
        }

        public TimelineKnob() : this(null) { }
        public TimelineKnob(string labelText) : base(labelText, new VisualElement())
        {
            if (knobImage == null) knobImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UITK_Simple_Timeline/UITK_SimpleTimelineIcons/timelinediamond.png");
            if (knobImage == null) Debug.LogError("TimelineKnob.cs can't find timelinediamond.png. Did you move the UITK_Simple_Timeline from your root Asset folder?");
            style.backgroundImage = knobImage;
            style.position = Position.Absolute;
            style.height = 25f;
            style.width = 25f;
        }
        public TimelineKnob(string labelText, TimelineKeyframe<T> kf, SerializedProperty knobProperty) : base(labelText, new VisualElement())
        {
            value = kf;
            if (knobImage == null) knobImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UITK_Simple_Timeline/UITK_SimpleTimelineIcons/timelinediamond.png");
            if (knobImage == null) Debug.LogError("TimelineKnob.cs can't find timelinediamond.png. Did you move the UITK_Simple_Timeline from your root Asset folder?");
            style.backgroundImage = knobImage;
            style.position = Position.Absolute;
            style.height = 25f;
            style.width = 25f;
            KnobProperty = knobProperty;
        }
    }
}
#endif