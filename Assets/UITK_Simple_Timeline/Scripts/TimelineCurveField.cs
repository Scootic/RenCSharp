#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
namespace UITK_SimpleTimeline
{
    [UxmlElement]
    public partial class TestTimelineCurveField : TimelineCurveField<Vector3, string>
    {

    }

    /// <summary>
    /// god
    /// </summary>
    /// <typeparam name="T">The values that are lerped</typeparam>
    /// <typeparam name="U">The object that is affected</typeparam>
    [UxmlElement]
    public partial class TimelineCurveField<T,U> : BaseField<TypedTimelineCurve<T,U>> where U : class
    {
        //public Action DeleteMeAction;
        protected readonly Dictionary<float,TimelineKnob<T>> KeyframeIcons;
        
        protected readonly VisualElement CurveDataContainer, KeyframeContainer;
        protected VisualElement o;
        /// <summary>
        /// Should hold the data for the TimelineCurve's U value.
        /// </summary>
        protected readonly PropertyField ToBeAffectedField;
        protected GenericMenu AddNewKeyframeMenu, DeleteCurveMenu;

        protected SerializedProperty curveProperty;
        protected readonly int myPropertyIndex;

        public TimelineCurveField() : this(null) { }
        //grumpus constructor that's bad!
        public TimelineCurveField(string labelText) : base(labelText, new VisualElement())
        {
            //value = curve;
            //myPropertyIndex = index;
            Remove(Children().ToArray()[0]);
            KeyframeIcons = new();
            style.height = 150;
            style.right = 0;
            style.left = -155;
            style.top = 0;
            style.bottom = Helper.Auto;
            style.flexGrow = 1;
            style.flexShrink = -1;
            style.maxHeight = 150;
            style.width = Helper.Auto;
            style.minWidth = 150;
            style.maxWidth = 9999999999;
            style.backgroundColor = Helper.SecondLayerBG;
            style.borderBottomColor = Helper.SecondLayerBorder;
            style.borderLeftColor = Helper.SecondLayerBorder;
            style.borderTopColor = Helper.SecondLayerBorder;
            style.borderRightColor = Helper.SecondLayerBorder;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
            style.borderTopWidth = 1;
            style.borderLeftWidth = 1;
            style.flexDirection = FlexDirection.Row;
            style.position = Position.Absolute;

            CurveDataContainer = new() { name = "CurveDataContainer" };
            CurveDataContainer.style.width = 150;
            CurveDataContainer.style.minWidth = 150;
            CurveDataContainer.style.maxWidth = 150;
            CurveDataContainer.style.left = 0;
            CurveDataContainer.style.right = 5;
            CurveDataContainer.style.height = 150;
            CurveDataContainer.style.flexGrow = 0;
            CurveDataContainer.style.flexShrink = 1;
            CurveDataContainer.style.backgroundColor = Helper.SecondLayerBG;
            CurveDataContainer.style.borderBottomColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderRightColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderTopColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderLeftColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderBottomWidth = 1;
            CurveDataContainer.style.borderRightWidth = 1;
            CurveDataContainer.style.borderTopWidth = 1;
            CurveDataContainer.style.borderLeftWidth = 1;
            Add(CurveDataContainer);

            ToBeAffectedField = new() { name = "ToBeAffectedField" };
            //curveProperty = Helper.CurvesProperty.GetArrayElementAtIndex(index);
            ToBeAffectedField.style.width = 150;
            ToBeAffectedField.style.height = 150;
            //ToBeAffectedField.BindProperty(curveProperty.FindPropertyRelative("ToAffect")); //?

            ToBeAffectedField.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                ResizeLabel();
            });

            CurveDataContainer.Add(ToBeAffectedField);

            KeyframeContainer = new() { name = "KeyframeContainer" };
            KeyframeContainer.style.left = 151;
            KeyframeContainer.style.right = -150;
            KeyframeContainer.style.height = 150;
            KeyframeContainer.style.maxHeight = 150;
            KeyframeContainer.style.position = Position.Absolute;
            KeyframeContainer.style.backgroundColor = Helper.SecondLayerBorder;
            KeyframeContainer.style.backgroundImage = Helper.FullRulerLength;
            KeyframeContainer.style.unityBackgroundImageTintColor = Helper.HalfTransparentWhite;
            KeyframeContainer.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 0f));
            KeyframeContainer.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            KeyframeContainer.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            KeyframeContainer.style.flexGrow = 1;
            KeyframeContainer.style.flexShrink = -1;
            Add(KeyframeContainer);

            SpawnKeyframeKnobs(value);
            RegisterGenericMenus();
        }
        public TimelineCurveField(string labelText, TypedTimelineCurve<T, U> curve, int index) : base(labelText, new VisualElement())
        {
            value = curve;
            myPropertyIndex = index;
            Remove(Children().ToArray()[0]);
            KeyframeIcons = new();
            style.height = 150;
            style.right = 0;
            style.left = -155;
            style.top = 150 * index + 25;
            style.bottom = Helper.Auto;
            style.flexGrow = 1;
            style.flexShrink = -1;
            style.maxHeight = 150;
            style.width = Helper.Auto;
            style.minWidth = 150;
            style.maxWidth = 9999999999;
            style.backgroundColor = Helper.SecondLayerBG;
            style.borderBottomColor = Helper.SecondLayerBorder;
            style.borderLeftColor = Helper.SecondLayerBorder;
            style.borderTopColor = Helper.SecondLayerBorder;
            style.borderRightColor = Helper.SecondLayerBorder;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
            style.borderTopWidth = 1;
            style.borderLeftWidth = 1;
            style.flexDirection = FlexDirection.Row;
            style.position = Position.Absolute;

            CurveDataContainer = new() { name = "CurveDataContainer" };
            CurveDataContainer.style.width = 150;
            CurveDataContainer.style.minWidth = 150;
            CurveDataContainer.style.maxWidth = 150;
            CurveDataContainer.style.left = 0;
            CurveDataContainer.style.right = 5;
            CurveDataContainer.style.height = 150;
            CurveDataContainer.style.flexGrow = 0;
            CurveDataContainer.style.flexShrink = 1;
            CurveDataContainer.style.backgroundColor = Helper.SecondLayerBG;
            CurveDataContainer.style.borderBottomColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderRightColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderTopColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderLeftColor = Helper.SecondLayerBorder;
            CurveDataContainer.style.borderBottomWidth = 1;
            CurveDataContainer.style.borderRightWidth = 1;
            CurveDataContainer.style.borderTopWidth = 1;
            CurveDataContainer.style.borderLeftWidth = 1;
            Add(CurveDataContainer);

            ToBeAffectedField = new() { name = "ToBeAffectedField" };
            ToBeAffectedField.RemoveFromClassList(alignedFieldUssClassName);
            curveProperty = Helper.CurvesProperty.GetArrayElementAtIndex(index);
            ToBeAffectedField.style.width = 150;
            ToBeAffectedField.style.height = 150;
            ToBeAffectedField.style.flexWrap = Wrap.Wrap;
            ToBeAffectedField.style.flexGrow = 1;
            //RegisterCallback<GeometryChangedEvent>(evt =>
            //{
            //    ResizeLabel();
            //});
            CurveDataContainer.Add(ToBeAffectedField);
            ToBeAffectedField.BindProperty(curveProperty.FindPropertyRelative("ToAffect")); //?    

            KeyframeContainer = new() { name = "KeyframeContainer" };
            KeyframeContainer.style.left = 151;
            KeyframeContainer.style.right = -150;
            KeyframeContainer.style.height = 150;
            KeyframeContainer.style.maxHeight = 150;
            KeyframeContainer.style.position = Position.Absolute;
            KeyframeContainer.style.backgroundColor = Helper.SecondLayerBorder;
            KeyframeContainer.style.backgroundImage = Helper.FullRulerLength;
            KeyframeContainer.style.unityBackgroundImageTintColor = Helper.HalfTransparentWhite;
            KeyframeContainer.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 0f));
            KeyframeContainer.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            KeyframeContainer.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            KeyframeContainer.style.flexGrow = 1;
            KeyframeContainer.style.flexShrink = -1;
            Add(KeyframeContainer);

            SpawnKeyframeKnobs(value);
            RegisterGenericMenus();
            MarkDirtyRepaint();

            schedule.Execute(ResizeLabel).Until(() => o != null);
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

        protected void SpawnKeyframeKnobs(TypedTimelineCurve<T, U> curve)
        {
            if (curve == null) return;
            for(int i = 0; i < curve.Keyframes.Count; i++)
            {
                TimelineKeyframe<T> kf = curve.Keyframes[i];
                TimelineKnob<T> tKnob = new("", kf, curveProperty.FindPropertyRelative("keyframes").GetArrayElementAtIndex(i));
                tKnob.transform.position = new Vector3(Helper.PixelWidthPerSeconds * kf.Time, 0, 0);
                tKnob.DeleteKnobAction += delegate
                {
                    KeyframeIcons.Remove(kf.Time);
                    curve.RemoveKeyframeFromCurve(kf.Time);
                };
                tKnob.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.button == 1) tKnob.DeleteMe.ShowAsContext();
                    else if (evt.button == 0) Helper.ReceiveKeyframe?.Invoke(tKnob.KnobProperty);
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
                    float tToAddAt = evt.localPosition.x / Helper.PixelWidthPerSeconds;
                    tToAddAt = (float)Math.Round(tToAddAt,1, MidpointRounding.AwayFromZero);
                    AddNewKeyframeMenu = new();
                    AddNewKeyframeMenu.AddItem(new GUIContent($"Add Keyframe ({value.SpawnKeyframeName()}) at {tToAddAt}"), false, delegate
                    {
                        value.AddKeyframeToCurve(tToAddAt);
                        RegenerateIcons();
                    });
                    AddNewKeyframeMenu.ShowAsContext();
                }
                evt.StopPropagation();
            });

            CurveDataContainer.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1)
                {
                    DeleteCurveMenu = new();
                    DeleteCurveMenu.AddItem(new GUIContent($"Delete Curve {myPropertyIndex}: {value.DeleteCurveName()}?!?"), false, delegate
                    {
                        Helper.RemoveTimelineCurve?.Invoke(curveProperty.managedReferenceValue as TimelineCurve);
                    });
                    DeleteCurveMenu.ShowAsContext();
                    evt.StopPropagation();
                }
            });
        }

        protected void ResizeLabel()
        {
            try
            {
                o = ToBeAffectedField.Children().ToArray()[0];
                o.style.flexDirection = FlexDirection.Column;
                o.style.flexWrap = Wrap.Wrap;
                Label l = o.Q<Label>();
                l.text = value.ToAffectName();
                l.style.maxWidth = 145;
                l.style.minWidth = 50;
                l.style.flexGrow = -1;
                l.style.flexShrink = 1;
                VisualElement v = o.Children().ToArray()[1];
                v.style.flexGrow = 1;
                v.style.minHeight = 17;
                v.style.minWidth = 90;
                v.style.maxWidth = 145;
            }
            catch
            {
                //Log in-case you have dire expectations.
                //Debug.LogWarning("Stupid ah label couldnae be found from ToBeAffectedField >:(");
            }
        }
    }
}
#endif