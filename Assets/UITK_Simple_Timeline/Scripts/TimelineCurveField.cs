using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// god
    /// </summary>
    /// <typeparam name="T">The values that are lerped</typeparam>
    /// <typeparam name="U">The object that is affected</typeparam>
    [UxmlElement]
    public partial class TimelineCurveField<T,U> : BaseField<TimelineCurve<T,U>> where U : UnityEngine.Object
    {
        //public Action DeleteMeAction;
        protected readonly Dictionary<float,TimelineKnob<T>> KeyframeIcons;
        protected readonly DestroyableVisualElement me;
        
        protected readonly VisualElement CurveDataContainer;
        protected readonly VisualElement KeyframeContainer;
        /// <summary>
        /// Should hold the data for the TimelineCurve's U value.
        /// </summary>
        protected readonly PropertyField ToBeAffectedField;
        protected GenericMenu AddNewKeyframeMenu, DeleteCurveMenu;

        public TimelineCurveField() : this(null) { }
        //grumpus constructor that's bad!
        public TimelineCurveField(string labelText) : base(labelText, new DestroyableVisualElement())
        {
            me = this as DestroyableVisualElement;
            KeyframeIcons = new();

            style.height = 150;
            style.left = 0;
            style.right = 0;
            style.flexGrow = 1;
            style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBG;
            style.borderBottomColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            style.borderLeftColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            style.borderTopColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            style.borderRightColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
            style.borderTopWidth = 1;
            style.borderLeftWidth = 1;
            style.flexDirection = FlexDirection.Row;

            CurveDataContainer = new();
            CurveDataContainer.style.width = 150;
            CurveDataContainer.style.height = 150;
            CurveDataContainer.style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBG;
            CurveDataContainer.style.borderBottomColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            CurveDataContainer.style.borderRightColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            CurveDataContainer.style.borderTopColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            CurveDataContainer.style.borderLeftColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            CurveDataContainer.style.borderBottomWidth = 1;
            CurveDataContainer.style.borderRightWidth = 1;
            CurveDataContainer.style.borderTopWidth = 1;
            CurveDataContainer.style.borderLeftWidth = 1;
            Add(CurveDataContainer);

            ToBeAffectedField = new();
            ToBeAffectedField.style.width = 150;
            ToBeAffectedField.style.height = 150;
            //ToBeAffectedField.BindProperty(new SerializedObject(curve.ToAffect)); //?
            CurveDataContainer.Add(ToBeAffectedField);

            KeyframeContainer = new();
            KeyframeContainer.style.left = 0;
            KeyframeContainer.style.right = 0;
            KeyframeContainer.style.height = 150;
            Add(KeyframeContainer);

            SpawnKeyframeKnobs(value);
            RegisterGenericMenus();
        }
        public TimelineCurveField(string labelText, TimelineCurve<T, U> curve) : base(labelText, new DestroyableVisualElement())
        {
            me = this as DestroyableVisualElement;
            value = curve;
            KeyframeIcons = new();
            style.height = 150;
            style.left = 0;
            style.right = 0;
            style.flexGrow = 1;
            style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBG;
            style.borderBottomColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            style.borderLeftColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            style.borderTopColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            style.borderRightColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
            style.borderTopWidth = 1;
            style.borderLeftWidth = 1;
            style.flexDirection = FlexDirection.Row;

            CurveDataContainer = new();
            CurveDataContainer.style.width = 150;
            CurveDataContainer.style.height = 150;
            CurveDataContainer.style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBG;
            CurveDataContainer.style.borderBottomColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            CurveDataContainer.style.borderRightColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            CurveDataContainer.style.borderTopColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            CurveDataContainer.style.borderLeftColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            CurveDataContainer.style.borderBottomWidth = 1;
            CurveDataContainer.style.borderRightWidth = 1;
            CurveDataContainer.style.borderTopWidth = 1;
            CurveDataContainer.style.borderLeftWidth = 1;
            Add(CurveDataContainer);

            ToBeAffectedField = new();
            ToBeAffectedField.style.width = 150;
            ToBeAffectedField.style.height = 150;
            U gus = new UnityEngine.Object() as U;
            curve.ToAffect = gus;
            ToBeAffectedField.BindProperty(new SerializedObject(curve.ToAffect)); //?
            CurveDataContainer.Add(ToBeAffectedField);

            KeyframeContainer = new();
            KeyframeContainer.style.left = 0;
            KeyframeContainer.style.right = 0;
            KeyframeContainer.style.height = 150;
            Add(KeyframeContainer);

            SpawnKeyframeKnobs(value);
            RegisterGenericMenus();
        }

        protected void RegenerateIcons()
        {
            //go through elements of the list, place them around, and make sure they've got values 'n' shite
            foreach (KeyValuePair<float, TimelineKnob<T>> kvp in KeyframeIcons)
            {
                KeyframeContainer.Remove(kvp.Value);
            }
            KeyframeIcons.Clear();
            SpawnKeyframeKnobs(value);
        }

        protected void SpawnKeyframeKnobs(TimelineCurve<T, U> curve)
        {
            foreach (TimelineKeyframe<T> kf in curve.Keyframes)
            {
                TimelineKnob<T> tKnob = new("", kf);
                tKnob.transform.position = new Vector3(SimpleTimelineUITK_Helper.PixelWidthPerSeconds * kf.Time, 0, 0);
                tKnob.DeleteKnobAction += delegate
                {
                    KeyframeIcons.Remove(kf.Time);
                    curve.RemoveKeyframeFromCurve(kf.Time);
                };
                tKnob.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.button == 1) tKnob.DeleteMe.ShowAsContext();
                    else if (evt.button == 0) SimpleTimelineUITK_Helper.ReceiveKeyframe?.Invoke(tKnob.value as TimelineKeyframe<object>);
                });
                KeyframeIcons.Add(kf.Time, tKnob);
                KeyframeContainer.Add(tKnob);
            }
        }

        protected void RegisterGenericMenus()
        {
            KeyframeContainer.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1) //if right click, spawn and set generic menu, get time to add based on mouse pos?
                {
                    float tToAddAt = evt.localPosition.x / SimpleTimelineUITK_Helper.PixelWidthPerSeconds;
                    AddNewKeyframeMenu = new();
                    AddNewKeyframeMenu.AddItem(new GUIContent($"Add Keyframe at {tToAddAt}"), false, delegate
                    {
                        value.AddKeyframeToCurve(tToAddAt);
                        RegenerateIcons();
                    });
                    AddNewKeyframeMenu.ShowAsContext();
                }
            });

            CurveDataContainer.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1)
                {
                    DeleteCurveMenu = new();
                    DeleteCurveMenu.AddItem(new GUIContent("Delete Curve?!?"), false, delegate
                    {
                        RemoveFromHierarchy();
                        //more clean up required?!?
                        me.DeleteMe?.Invoke();
                    });
                    DeleteCurveMenu.ShowAsContext();
                }
            });
        }
    }
}
