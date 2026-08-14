using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK_SimpleTimeline.Editor
{
    [UxmlElement]
    public partial class TimelineKnob<T> : BaseField<TimelineKeyframe<T>>
    {
        protected TimelineKnob<T> clipboard = null;
        public Action DeleteKnobAction;
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
            style.backgroundImage = Resources.Load<Texture2D>("UITK_SimpleTimeline_Icons/editordiamond.png");
            style.position = Position.Absolute;
            style.height = 25f;
            style.width = 25f;
        }
        public TimelineKnob(string labelText, TimelineKeyframe<T> kf) : base(labelText, new VisualElement())
        {
            value = kf;
            style.backgroundImage = Resources.Load<Texture2D>("UITK_SimpleTimeline_Icons/editordiamond.png");
            style.position = Position.Absolute;
            style.height = 25f;
            style.width = 25f;
        }
    }
}
