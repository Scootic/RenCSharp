#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
using System;
namespace UITK_SimpleTimeline
{
    [UxmlElement]
    public partial class SimpleTimelineUITKField : BaseField<SimpleTimeline>
    {
        protected readonly VisualElement Container;
        protected readonly IntegerField CurrentFrameField;

        protected readonly SimpleTimeline thisTimeline;

        //the scrollable area
        protected readonly ScrollView TimelineField;
        //knob property viewer
        protected readonly PropertyField CurrentKnobField;
        /// <summary>
        /// Please override with the types of curved knobs you want to be usable in your field.
        /// </summary>
        /// <returns></returns>
        protected virtual Type[] GetValidCurveTypes() { Debug.LogWarning("No valid types of knobs assigned."); return null; }


        public SimpleTimelineUITKField() : this(null) { }

        public SimpleTimelineUITKField(string labelText) : base(labelText, new VisualElement())
        {

        }

        public SimpleTimelineUITKField(string labelText, SimpleTimeline st) : base(labelText, new VisualElement())
        {
            thisTimeline = st;
        }
    }
}
#endif