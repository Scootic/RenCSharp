using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK_SimpleTimeline.Editor
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The values that are lerped</typeparam>
    /// <typeparam name="U">The object that is affected</typeparam>
    [UxmlElement]
    public partial class TimelineCurveField<T, U> : BaseField<TimelineCurve<T, U>> where U : class
    {
        protected readonly Dictionary<float, TimelineKnob<T>> KeyframeIcons;
        protected readonly VisualElement Container;
        protected readonly float xOffset = 30; //?
        /// <summary>
        /// Should hold the data for the TimelineCurve's U value.
        /// </summary>
        protected readonly PropertyField ToBeAffectedField;
        protected GenericMenu AddNewKeyframeMenu;

        public TimelineCurveField() : this(null) { }
        public TimelineCurveField(string labelText) : base(labelText, new VisualElement())
        {
            Container = new VisualElement();
            KeyframeIcons = new();
            RegisterGenericMenu();
        }
        public TimelineCurveField(string labelText, TimelineCurve<T, U> curve) : base(labelText, new VisualElement())
        {
            value = curve;
            Container = new VisualElement();
            KeyframeIcons = new();

            SpawnKeyframeKnobs(value);
            RegisterGenericMenu();

            foreach (KeyValuePair<float, TimelineKnob<T>> kvp in KeyframeIcons)
            {
                Container.Add(kvp.Value);
            }
        }

        protected void RegenerateIcons()
        {
            //go through elements of the list, place them around, and make sure they've got values 'n' shite
            foreach (KeyValuePair<float, TimelineKnob<T>> kvp in KeyframeIcons)
            {
                Container.Remove(kvp.Value);
            }
            KeyframeIcons.Clear();
            SpawnKeyframeKnobs(value);
        }

        protected void SpawnKeyframeKnobs(TimelineCurve<T, U> curve)
        {
            foreach (TimelineKeyframe<T> kf in curve.Keyframes)
            {
                TimelineKnob<T> tKnob = new("", kf);
                tKnob.transform.position = new Vector3(30 * kf.Time, 0, 0);
                tKnob.DeleteKnobAction += delegate
                {
                    KeyframeIcons.Remove(kf.Time);
                    curve.RemoveKeyframeFromCurve(kf.Time);
                };
                tKnob.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.button == 1) tKnob.DeleteMe.ShowAsContext();
                });
                KeyframeIcons.Add(kf.Time, tKnob);
            }
        }

        protected void RegisterGenericMenu()
        {
            Container.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1) //if right click, spawn and set generic menu, get time to add based on mouse pos?
                {
                    AddNewKeyframeMenu = new();
                }
            });
        }
    }
}
