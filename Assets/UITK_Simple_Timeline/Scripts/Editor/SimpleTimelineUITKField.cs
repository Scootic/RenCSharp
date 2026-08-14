#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    /// <summary>
    /// Despite being a partial class, please consider this to be abstract and not usable. A Visual Editor to make changes
    /// and display a timeline window for SimpleTimelines.
    /// </summary>
    [UxmlElement]
    public partial class SimpleTimelineUITKField : BaseField<SimpleTimeline>
    {
        protected readonly VisualElement Container, LeftControlPanel, CurrentTimePreview;
        protected readonly IntegerField CurrentFrameField;

        protected readonly SimpleTimeline thisTimeline;

        //the scrollable area
        protected readonly ScrollView TimelineField;
        //knob property viewer
        protected readonly PropertyField CurrentKeyframeField;
        //? the guys that'll be displayed in the scroll view?
        protected readonly List<TimelineCurveField<object,object>>TimelineCurveFields;
        //boy that'll show up when user right clicks inside curvey area?
        protected GenericMenu AddNewCurveMenu;
        /// <summary>
        /// Please override with the types of curved knobs you want to be usable in your field.
        /// </summary>
        /// <returns></returns>
        protected virtual Type[] GetValidCurveTypes() { Debug.LogWarning("No valid types of curves assigned."); return null; }


        public SimpleTimelineUITKField() : this(null) { }

        public SimpleTimelineUITKField(string labelText) : base(labelText, new VisualElement())
        {

        }

        public SimpleTimelineUITKField(string labelText, SimpleTimeline st) : base(labelText, new VisualElement())
        {
            thisTimeline = st;



            CurrentFrameField.value = 0;
        }
    }
}
#endif