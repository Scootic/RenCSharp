#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
using UnityEditor.UIElements;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// guh
    /// </summary>
    /// <typeparam name="T">The values that are lerped</typeparam>
    [UxmlElement]
    public partial class DoubleTypedArrayTimelineCurveField<T, U> : BaseField<ArrayTimelineCurve<T, U>>, IRepresentValue, IRegeneratableElement where T : notnull where U : notnull
    {
        //public Action DeleteMeAction;
        protected readonly Dictionary<float, TimelineKnob<T>>[] KeyframeIcons;

        protected readonly VisualElement KeyframeContainer;
        protected readonly ScrollView CurveDataContainer;
        protected readonly Label TypeLabel;
        protected readonly PropertyField WrapModeField, ToBeAffectedField;
        protected readonly ScalingPropertyField PlaybackRepField;
        protected GenericMenu AddNewKeyframeMenu, DeleteCurveMenu;

        protected SerializedProperty curveProperty, keyframesProperty, playbackProperty;
        protected readonly int myPropertyIndex;
        protected VisualElement o, w;

        public DoubleTypedArrayTimelineCurveField() : this(null) { }
        //grumpus constructor that's bad!
        public DoubleTypedArrayTimelineCurveField(string labelText) : base(labelText, new VisualElement())
        {
            //value = curve;
            //myPropertyIndex = index;
            Remove(Children().ToArray()[0]);
            //KeyframeIcons = new();
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
            style.backgroundColor = Helper.DefaultSecondLayerBG;
            style.borderBottomColor = Helper.DefaultSecondLayerBorder;
            style.borderLeftColor = Helper.DefaultSecondLayerBorder;
            style.borderTopColor = Helper.DefaultSecondLayerBorder;
            style.borderRightColor = Helper.DefaultSecondLayerBorder;
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
            CurveDataContainer.style.backgroundColor = Helper.DefaultSecondLayerBG;
            CurveDataContainer.style.borderBottomColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderRightColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderTopColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderLeftColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderBottomWidth = 1;
            CurveDataContainer.style.borderRightWidth = 1;
            CurveDataContainer.style.borderTopWidth = 1;
            CurveDataContainer.style.borderLeftWidth = 1;
            Add(CurveDataContainer);

            KeyframeContainer = new() { name = "KeyframeContainer" };
            KeyframeContainer.style.left = 151;
            KeyframeContainer.style.right = -150;
            KeyframeContainer.style.height = 150;
            KeyframeContainer.style.maxHeight = 150;
            KeyframeContainer.style.position = Position.Absolute;
            KeyframeContainer.style.backgroundColor = Helper.DefaultSecondLayerBorder;
            KeyframeContainer.style.backgroundImage = Helper.FullRulerLength;
            KeyframeContainer.style.unityBackgroundImageTintColor = Helper.HalfTransparentWhite;
            KeyframeContainer.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 0f));
            KeyframeContainer.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            KeyframeContainer.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            KeyframeContainer.style.flexGrow = 1;
            KeyframeContainer.style.flexShrink = -1;
            Add(KeyframeContainer);

            SpawnKeyframeKnobs();
            RegisterGenericMenus();
        }
        public DoubleTypedArrayTimelineCurveField(string labelText, ArrayTimelineCurve<T, U> curve, int index) : base(labelText, new VisualElement())
        {
            if (curve.Keyframes.Length != curve.DefaultArrayLength()) curve.SetKeyframesArrayLength = curve.DefaultArrayLength();
            value = curve;
            myPropertyIndex = index;
            Remove(Children().ToArray()[0]);
            KeyframeIcons = new Dictionary<float, TimelineKnob<T>>[value.Keyframes.Length];
            for (int i = 0; i < KeyframeIcons.Length; i++)
            {
                KeyframeIcons[i] = new();
            }
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
            style.backgroundColor = Helper.DefaultSecondLayerBG;
            style.borderBottomColor = Helper.DefaultSecondLayerBorder;
            style.borderLeftColor = Helper.DefaultSecondLayerBorder;
            style.borderTopColor = Helper.DefaultSecondLayerBorder;
            style.borderRightColor = Helper.DefaultSecondLayerBorder;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
            style.borderTopWidth = 1;
            style.borderLeftWidth = 1;
            style.flexDirection = FlexDirection.Row;
            style.position = Position.Absolute;

            CurveDataContainer = new() { name = "CurveDataContainer" };
            CurveDataContainer.style.width = 160;
            CurveDataContainer.style.minWidth = 160;
            CurveDataContainer.style.maxWidth = 160;
            CurveDataContainer.style.left = -25;
            CurveDataContainer.style.right = 25;
            CurveDataContainer.style.height = 150;
            CurveDataContainer.style.flexGrow = 1;
            CurveDataContainer.style.flexShrink = 1;
            CurveDataContainer.style.backgroundColor = Helper.DefaultSecondLayerBG;
            CurveDataContainer.style.borderBottomColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderRightColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderTopColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderLeftColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderBottomWidth = 1;
            CurveDataContainer.style.borderRightWidth = 1;
            CurveDataContainer.style.borderTopWidth = 1;
            CurveDataContainer.style.borderLeftWidth = 1;
            Add(CurveDataContainer);

            TypeLabel = new() { name = "TypeLabel", text = value.ShorthandCurveName() };
            TypeLabel.style.left = 25;
            TypeLabel.style.right = -25;
            TypeLabel.style.flexWrap = Wrap.Wrap;
            TypeLabel.style.maxHeight = 30f;
            TypeLabel.style.maxWidth = 125;
            TypeLabel.style.whiteSpace = WhiteSpace.Normal;
            CurveDataContainer.Add(TypeLabel);

            curveProperty = Helper.CurvesProperty.GetArrayElementAtIndex(index);
            keyframesProperty = curveProperty.FindPropertyRelative("keyframes");
            playbackProperty = curveProperty.FindPropertyRelative("TemporaryRep");

            ToBeAffectedField = new() { name = "ToBeAffectedField" };
            ToBeAffectedField.RemoveFromClassList(alignedFieldUssClassName);
            curveProperty = Helper.CurvesProperty.GetArrayElementAtIndex(index);
            keyframesProperty = curveProperty.FindPropertyRelative("keyframes");
            ToBeAffectedField.style.width = 125;
            ToBeAffectedField.style.height = 40;
            ToBeAffectedField.style.left = 25;
            ToBeAffectedField.style.right = -25;
            ToBeAffectedField.style.flexWrap = Wrap.Wrap;
            ToBeAffectedField.style.flexGrow = 1;
            CurveDataContainer.Add(ToBeAffectedField);
            ToBeAffectedField.BindProperty(curveProperty.FindPropertyRelative("ToAffect")); //?

            WrapModeField = new() { name = "WrapModeField" };
            WrapModeField.RemoveFromClassList(alignedFieldUssClassName);
            WrapModeField.style.width = 125;
            WrapModeField.style.left = 25;
            WrapModeField.style.right = -25;
            WrapModeField.style.height = 40;
            WrapModeField.style.flexGrow = 1;
            CurveDataContainer.Add(WrapModeField);
            WrapModeField.BindProperty(curveProperty.FindPropertyRelative("WrappingMode"));

            playbackProperty.arraySize = curve.DefaultArrayLength();
            playbackProperty.serializedObject.ApplyModifiedProperties();
            playbackProperty.serializedObject.Update();
            PlaybackRepField = new(125, playbackProperty) { name = "PlaybackField" };
            PlaybackRepField.style.left = 25;
            PlaybackRepField.style.right = -25;
            CurveDataContainer.Add(PlaybackRepField);
            PlaybackRepField.TrackPropertyValue(playbackProperty, delegate
            {
                if (!Helper.Recording) return;//if not recording, do nothing
                for (int i = 0; i < playbackProperty.arraySize; i++)
                {
                    T bv = (T)playbackProperty.GetArrayElementAtIndex(i).boxedValue;
                    if (bv.Equals(value.EvaluateValue(Helper.CurT)[i])) continue;//if it's an expected value on the curve, don't care
                    if (!KeyframeIcons[i].ContainsKey(Helper.CurT))//if we don't already have keyframe there
                    {
                        AddKeyframeAtTime(Helper.CurT,i, bv);
                    }
                    else
                    {
                        SetKeyframeAtTime(Helper.CurT, i, bv);
                    }
                }
            }
            );

            KeyframeContainer = new() { name = "KeyframeContainer" };
            KeyframeContainer.style.left = 151;
            KeyframeContainer.style.right = -150;
            KeyframeContainer.style.height = 150;
            KeyframeContainer.style.maxHeight = 150;
            KeyframeContainer.style.position = Position.Absolute;
            KeyframeContainer.style.backgroundColor = Helper.DefaultSecondLayerBorder;
            KeyframeContainer.style.backgroundImage = Helper.FullRulerLength;
            KeyframeContainer.style.unityBackgroundImageTintColor = Helper.HalfTransparentWhite;
            KeyframeContainer.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 0f));
            KeyframeContainer.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            KeyframeContainer.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            KeyframeContainer.style.flexGrow = 1;
            KeyframeContainer.style.flexShrink = -1;
            Add(KeyframeContainer);

            SpawnKeyframeKnobs();
            RegisterGenericMenus();
            MarkDirtyRepaint();

            schedule.Execute(ResizeLabel).Until(() => w != null);
        }

        public void RegenerateElement()
        {
            //remove all pre-existing timelineknobs before adding them again
            for (int i = 0; i < KeyframeIcons.Length; i++)
            {
                foreach (KeyValuePair<float, TimelineKnob<T>> kvp in KeyframeIcons[i])
                {
                    KeyframeContainer.Remove(kvp.Value);
                }
                KeyframeIcons[i].Clear();
            }
            //add knobs lmao
            SpawnKeyframeKnobs();
        }

        protected virtual void SpawnKeyframeKnobs()
        {
            if (keyframesProperty == null) return;

            for (int i = 0; i < keyframesProperty.arraySize; i++)
            {
                int oldIndex = i;
                for (int j = 0; j < keyframesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("List").arraySize; j++)
                {
                    TimelineKnob<T> tKnob = new("", keyframesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("List").GetArrayElementAtIndex(j),
                        j, myPropertyIndex);
                    float time = tKnob.value.Time;
                    if (KeyframeIcons[i].ContainsKey(time)) continue;
                    tKnob.transform.position = new Vector3(Helper.PixelWidthPerSeconds * time - tKnob.style.width.value.value * 0.5f - 2, 0, 0);
                    tKnob.style.top = 15 + (15 * i);
                    tKnob.DeleteKnobAction += delegate
                    {
                        KeyframeIcons[oldIndex][time].RemoveFromHierarchy();
                        KeyframeIcons[oldIndex].Remove(time);
                        ArrayTimelineCurve<T,U> t = curveProperty.boxedValue as ArrayTimelineCurve<T,U>;
                        t.RemoveKeyframeFromCurve(time, oldIndex);
                        curveProperty.boxedValue = t;
                    };
                    tKnob.RegisterCallback<PointerDownEvent>(evt => //by some divine mercy, works.
                    {
                        if (evt.button == 1) tKnob.DeleteMe.ShowAsContext();
                        else if (evt.button == 0)
                        {
                            Helper.ReceiveKeyframe?.Invoke(tKnob.KnobProperty, tKnob, true);
                        }
                        evt.StopPropagation();
                    });

                    KeyframeIcons[i].Add(time, tKnob);
                    KeyframeContainer.Add(tKnob);
                }
            }
        }

        protected void RegisterGenericMenus()
        {
            KeyframeContainer.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1) //if right click, spawn and set generic menu, get time to add based on mouse pos?
                {
                    float tToAddAt = evt.localPosition.x / Helper.PixelWidthPerSeconds;
                    tToAddAt = (float)Math.Round(tToAddAt, 1, MidpointRounding.AwayFromZero);
                    AddNewKeyframeMenu = new();
                    for (int i = 0; i < keyframesProperty.arraySize; i++)
                    {
                        int index = i;
                        AddNewKeyframeMenu.AddItem(new GUIContent($"Add Keyframe ({value.SpawnKeyframeNames()[index]}) at {tToAddAt}"), false, delegate
                        {
                            AddKeyframeAtTime(tToAddAt, index);
                        });
                    }
                    AddNewKeyframeMenu.AddItem(new GUIContent($"Add All Keyframes at {tToAddAt}"), false, delegate
                    {
                        AddKeyframeAtTime(tToAddAt);
                    });
                    AddNewKeyframeMenu.AddSeparator("");
                    AddNewKeyframeMenu.AddItem(new GUIContent($"Closest Two Keyframes at {tToAddAt}"), false, delegate
                    {
                        string msg = $"The closest keyframes at {tToAddAt} are: ";
                        ArrayTimelineCurve<T,U> me = curveProperty.boxedValue as ArrayTimelineCurve<T,U>;
                        int[][] array = me.ArrayClosestTwoIndexes(tToAddAt);
                        for (int i = 0; i < array.Length; i++)
                        {
                            TimelineKeyframe<T> zero = me.Keyframes[i][array[i][0]];
                            TimelineKeyframe<T> one = me.Keyframes[i][array[i][1]];
                            Debug.Log($"\nLayer{i}: {array[i][0]} - {zero.Time} and {array[i][1]} - {one.Time}");
                        }
                        Debug.Log(msg);
                    });
                    AddNewKeyframeMenu.ShowAsContext();
                    evt.StopPropagation();
                }
            });

            CurveDataContainer.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1)
                {
                    DeleteCurveMenu = new();
                    DeleteCurveMenu.AddItem(new GUIContent($"Delete Curve {myPropertyIndex}: {value.ShorthandCurveName()}?!?"), false, delegate
                    {
                        Helper.RemoveTimelineCurve?.Invoke(curveProperty.boxedValue as TimelineCurve);
                    });
                    DeleteCurveMenu.ShowAsContext();

                    evt.StopPropagation();
                }
            });
        }

        public void AddKeyframeAtTime(float t)
        {
            (curveProperty.boxedValue as ArrayTimelineCurve<T,U>).AddKeyframeToCurve(t);
            Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
            RegenerateElement();
        }

        public void AddKeyframeAtTime(float t, int i)
        {
            (curveProperty.boxedValue as ArrayTimelineCurve<T,U>).AddKeyframeToCurve(t, i);
            Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
            RegenerateElement();
        }

        public void AddKeyframeAtTime(float t, int i, T value)
        {
            (curveProperty.boxedValue as ArrayTimelineCurve<T, U>).AddKeyframeToCurve(t, i, value);
            Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
            RegenerateElement();
        }

        public void SetKeyframeAtTime(float t, int i, T value)
        {
            (curveProperty.boxedValue as ArrayTimelineCurve<T, U>).SetKeyframeAtTime(t, i, value);
            Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
            RegenerateElement();
        }

        public void RepresentValue(float t)
        {
            for (int i = 0; i < playbackProperty.arraySize; i++)
            {
                playbackProperty.GetArrayElementAtIndex(i).boxedValue = (curveProperty.boxedValue as ArrayTimelineCurve<T,U>).EvaluateValue(t)[i];
            }
            playbackProperty.serializedObject.ApplyModifiedProperties();
            playbackProperty.serializedObject.Update();
            //Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
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
                l.style.maxWidth = 120;
                l.style.minWidth = 50;
                l.style.flexGrow = -1;
                l.style.flexShrink = 1;
                VisualElement v = o.Children().ToArray()[1];
                v.style.flexGrow = 1;
                v.style.minHeight = 17;
                v.style.minWidth = 90;
                v.style.maxWidth = 120;

                w = WrapModeField.Children().ToArray()[0];
                w.style.flexDirection = FlexDirection.Column;
                w.style.flexWrap = Wrap.Wrap;
                Label l2 = w.Q<Label>();
                l2.style.maxWidth = 120;
                l2.style.minWidth = 50;
                l2.style.flexGrow = -1;
                l2.style.flexShrink = -1;
                VisualElement v2 = w.Children().ToArray()[1];
                v2.style.flexGrow = 1;
                v2.style.minHeight = 17;
                v2.style.minWidth = 90;
                v2.style.maxWidth = 120;
            }
            catch
            {
                //Log in-case you have dire expectations. Will always fire because of how the stupid PropertyFields work,
                //but if the code actually works it will stop eventually and not just run forever.
                ///Debug.LogWarning("Stupid ah label couldnae be found from ToBeAffectedField >:(");
            }
        }
    }
    /// <summary>
    /// guh
    /// </summary>
    /// <typeparam name="T">The values that are lerped</typeparam>
    [UxmlElement]
    public partial class SingleTypedArrayTimelineCurveField<T> : BaseField<ArrayTimelineCurve<T>>, IRepresentValue, IRegeneratableElement where T : notnull
    {
        //public Action DeleteMeAction;
        protected readonly Dictionary<float, TimelineKnob<T>>[] KeyframeIcons;

        protected readonly VisualElement KeyframeContainer;
        protected readonly ScrollView CurveDataContainer;
        protected readonly Label TypeLabel;
        protected readonly PropertyField WrapModeField;
        protected readonly ScalingPropertyField PlaybackRepField;
        protected GenericMenu AddNewKeyframeMenu, DeleteCurveMenu;

        protected SerializedProperty curveProperty, keyframesProperty, playbackProperty;
        protected readonly int myPropertyIndex;
        protected VisualElement w;

        public SingleTypedArrayTimelineCurveField() : this(null) { }
        //grumpus constructor that's bad!
        public SingleTypedArrayTimelineCurveField(string labelText) : base(labelText, new VisualElement())
        {
            //value = curve;
            //myPropertyIndex = index;
            Remove(Children().ToArray()[0]);
            //KeyframeIcons = new();
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
            style.backgroundColor = Helper.DefaultSecondLayerBG;
            style.borderBottomColor = Helper.DefaultSecondLayerBorder;
            style.borderLeftColor = Helper.DefaultSecondLayerBorder;
            style.borderTopColor = Helper.DefaultSecondLayerBorder;
            style.borderRightColor = Helper.DefaultSecondLayerBorder;
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
            CurveDataContainer.style.backgroundColor = Helper.DefaultSecondLayerBG;
            CurveDataContainer.style.borderBottomColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderRightColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderTopColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderLeftColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderBottomWidth = 1;
            CurveDataContainer.style.borderRightWidth = 1;
            CurveDataContainer.style.borderTopWidth = 1;
            CurveDataContainer.style.borderLeftWidth = 1;
            Add(CurveDataContainer);

            KeyframeContainer = new() { name = "KeyframeContainer" };
            KeyframeContainer.style.left = 151;
            KeyframeContainer.style.right = -150;
            KeyframeContainer.style.height = 150;
            KeyframeContainer.style.maxHeight = 150;
            KeyframeContainer.style.position = Position.Absolute;
            KeyframeContainer.style.backgroundColor = Helper.DefaultSecondLayerBorder;
            KeyframeContainer.style.backgroundImage = Helper.FullRulerLength;
            KeyframeContainer.style.unityBackgroundImageTintColor = Helper.HalfTransparentWhite;
            KeyframeContainer.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 0f));
            KeyframeContainer.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            KeyframeContainer.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            KeyframeContainer.style.flexGrow = 1;
            KeyframeContainer.style.flexShrink = -1;
            Add(KeyframeContainer);

            SpawnKeyframeKnobs();
            RegisterGenericMenus();
        }
        public SingleTypedArrayTimelineCurveField(string labelText, ArrayTimelineCurve<T> curve, int index) : base(labelText, new VisualElement())
        {
            if (curve.Keyframes.Length != curve.DefaultArrayLength()) curve.SetKeyframesArrayLength = curve.DefaultArrayLength();
            value = curve;
            myPropertyIndex = index;
            Remove(Children().ToArray()[0]);
            KeyframeIcons = new Dictionary<float, TimelineKnob<T>>[value.Keyframes.Length];
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
            style.backgroundColor = Helper.DefaultSecondLayerBG;
            style.borderBottomColor = Helper.DefaultSecondLayerBorder;
            style.borderLeftColor = Helper.DefaultSecondLayerBorder;
            style.borderTopColor = Helper.DefaultSecondLayerBorder;
            style.borderRightColor = Helper.DefaultSecondLayerBorder;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
            style.borderTopWidth = 1;
            style.borderLeftWidth = 1;
            style.flexDirection = FlexDirection.Row;
            style.position = Position.Absolute;

            CurveDataContainer = new() { name = "CurveDataContainer" };
            CurveDataContainer.style.width = 160;
            CurveDataContainer.style.minWidth = 160;
            CurveDataContainer.style.maxWidth = 160;
            CurveDataContainer.style.left = -25;
            CurveDataContainer.style.right = 25;
            CurveDataContainer.style.height = 150;
            CurveDataContainer.style.flexGrow = 1;
            CurveDataContainer.style.flexShrink = 1;
            CurveDataContainer.style.backgroundColor = Helper.DefaultSecondLayerBG;
            CurveDataContainer.style.borderBottomColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderRightColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderTopColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderLeftColor = Helper.DefaultSecondLayerBorder;
            CurveDataContainer.style.borderBottomWidth = 1;
            CurveDataContainer.style.borderRightWidth = 1;
            CurveDataContainer.style.borderTopWidth = 1;
            CurveDataContainer.style.borderLeftWidth = 1;
            Add(CurveDataContainer);

            TypeLabel = new() { name = "TypeLabel", text = value.ShorthandCurveName() };
            TypeLabel.style.left = 25;
            TypeLabel.style.right = -25;
            TypeLabel.style.flexWrap = Wrap.Wrap;
            TypeLabel.style.maxHeight = 30f;
            TypeLabel.style.maxWidth = 125;
            TypeLabel.style.whiteSpace = WhiteSpace.Normal;
            CurveDataContainer.Add(TypeLabel);

            curveProperty = Helper.CurvesProperty.GetArrayElementAtIndex(index);
            keyframesProperty = curveProperty.FindPropertyRelative("keyframes");
            playbackProperty = curveProperty.FindPropertyRelative("TemporaryRep");

            WrapModeField = new() { name = "WrapModeField" };
            WrapModeField.RemoveFromClassList(alignedFieldUssClassName);
            WrapModeField.style.width = 125;
            WrapModeField.style.left = 25;
            WrapModeField.style.right = -25;
            WrapModeField.style.height = 40;
            WrapModeField.style.flexGrow = 1;
            CurveDataContainer.Add(WrapModeField);
            WrapModeField.BindProperty(curveProperty.FindPropertyRelative("WrappingMode"));

            playbackProperty.arraySize = curve.DefaultArrayLength();
            playbackProperty.serializedObject.ApplyModifiedProperties();
            playbackProperty.serializedObject.Update();

            PlaybackRepField = new(125, playbackProperty) { name = "PlaybackField" };
            PlaybackRepField.style.left = 25;
            PlaybackRepField.style.right = -25;
            CurveDataContainer.Add(PlaybackRepField);
            PlaybackRepField.TrackPropertyValue(playbackProperty, delegate
            {
                if (!Helper.Recording) return;//if not recording, do nothing
                for (int i = 0; i < playbackProperty.arraySize; i++)
                {
                    T bv = (T)playbackProperty.GetArrayElementAtIndex(i).boxedValue;
                    if (bv.Equals(value.EvaluateValue(Helper.CurT)[i])) continue;//if it's an expected value on the curve, don't care
                    if (!KeyframeIcons[i].ContainsKey(Helper.CurT))//if we don't already have keyframe there
                    {
                        AddKeyframeAtTime(Helper.CurT, i, bv);
                    }
                    else
                    {
                        SetKeyframeAtTime(Helper.CurT, i, bv);
                    }
                }
            }
            );

            KeyframeContainer = new() { name = "KeyframeContainer" };
            KeyframeContainer.style.left = 151;
            KeyframeContainer.style.right = -150;
            KeyframeContainer.style.height = 150;
            KeyframeContainer.style.maxHeight = 150;
            KeyframeContainer.style.position = Position.Absolute;
            KeyframeContainer.style.backgroundColor = Helper.DefaultSecondLayerBorder;
            KeyframeContainer.style.backgroundImage = Helper.FullRulerLength;
            KeyframeContainer.style.unityBackgroundImageTintColor = Helper.HalfTransparentWhite;
            KeyframeContainer.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 0f));
            KeyframeContainer.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            KeyframeContainer.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            KeyframeContainer.style.flexGrow = 1;
            KeyframeContainer.style.flexShrink = -1;
            Add(KeyframeContainer);

            SpawnKeyframeKnobs();
            RegisterGenericMenus();
            MarkDirtyRepaint();

            schedule.Execute(ResizeLabel).Until(() => w != null);
        }

        public void RegenerateElement()
        {
            //remove all pre-existing timelineknobs before adding them again
            for (int i = 0; i < KeyframeIcons.Length; i++)
            {
                foreach (KeyValuePair<float, TimelineKnob<T>> kvp in KeyframeIcons[i])
                {
                    KeyframeContainer.Remove(kvp.Value);
                }
                KeyframeIcons[i].Clear();
            }
            //add knobs lmao
            SpawnKeyframeKnobs();
        }

        protected virtual void SpawnKeyframeKnobs()
        {
            if (keyframesProperty == null) return;

            for (int i = 0; i < keyframesProperty.arraySize; i++)
            {
                int oldIndex = i;
                for (int j = 0; j < keyframesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("List").arraySize; j++)
                {
                    TimelineKnob<T> tKnob = new("", keyframesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("List").GetArrayElementAtIndex(j),
                        j, myPropertyIndex);
                    float time = tKnob.value.Time;
                    tKnob.transform.position = new Vector3(Helper.PixelWidthPerSeconds * time - tKnob.style.width.value.value * 0.5f - 2, 0, 0);
                    tKnob.style.top = 15 + (15 * i);
                    tKnob.DeleteKnobAction += delegate
                    {
                        KeyframeIcons[oldIndex][time].RemoveFromHierarchy();
                        KeyframeIcons[oldIndex].Remove(time);
                        ArrayTimelineCurve<T> t = curveProperty.boxedValue as ArrayTimelineCurve<T>;
                        t.RemoveKeyframeFromCurve(time, oldIndex);
                        curveProperty.boxedValue = t;
                    };
                    tKnob.RegisterCallback<PointerDownEvent>(evt => //by some divine mercy, works.
                    {
                        if (evt.button == 1) tKnob.DeleteMe.ShowAsContext();
                        else if (evt.button == 0)
                        {
                            Helper.ReceiveKeyframe?.Invoke(tKnob.KnobProperty, tKnob, true);
                        }
                        evt.StopPropagation();
                    });
                    KeyframeIcons[i].Add(time, tKnob);
                    KeyframeContainer.Add(tKnob);
                }
            }
        }

        protected void RegisterGenericMenus()
        {
            KeyframeContainer.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1) //if right click, spawn and set generic menu, get time to add based on mouse pos?
                {
                    float tToAddAt = evt.localPosition.x / Helper.PixelWidthPerSeconds;
                    tToAddAt = (float)Math.Round(tToAddAt, 1, MidpointRounding.AwayFromZero);
                    AddNewKeyframeMenu = new();
                    for (int i = 0; i < keyframesProperty.arraySize; i++)
                    {
                        int index = i;
                        AddNewKeyframeMenu.AddItem(new GUIContent($"Add Keyframe ({value.SpawnKeyframeNames()[index]}) at {tToAddAt}"), false, delegate
                        {
                            AddKeyframeAtTime(tToAddAt, index);
                        });
                    }
                    AddNewKeyframeMenu.AddItem(new GUIContent($"Add All Keyframes at {tToAddAt}"), false, delegate
                    {
                        AddKeyframeAtTime(tToAddAt);
                    });
                    AddNewKeyframeMenu.AddSeparator("");
                    AddNewKeyframeMenu.AddItem(new GUIContent($"Closest Two Keyframes at {tToAddAt}"), false, delegate
                    {
                        string msg = $"The closest keyframes at {tToAddAt} are: ";
                        ArrayTimelineCurve<T> me = curveProperty.boxedValue as ArrayTimelineCurve<T>;
                        int[][] array = me.ArrayClosestTwoIndexes(tToAddAt);
                        for (int i = 0; i < array.Length; i++)
                        {
                            TimelineKeyframe<T> zero = me.Keyframes[i][array[i][0]];
                            TimelineKeyframe<T> one = me.Keyframes[i][array[i][1]];
                            Debug.Log($"\nLayer{i}: {array[i][0]} - {zero.Time} and {array[i][1]} - {one.Time}");
                        }
                        Debug.Log(msg);
                    });
                    AddNewKeyframeMenu.ShowAsContext();
                    evt.StopPropagation();
                }
            });

            CurveDataContainer.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1)
                {
                    DeleteCurveMenu = new();
                    DeleteCurveMenu.AddItem(new GUIContent($"Delete Curve {myPropertyIndex}: {value.ShorthandCurveName()}?!?"), false, delegate
                    {
                        Helper.RemoveTimelineCurve?.Invoke(curveProperty.boxedValue as TimelineCurve);
                    });
                    DeleteCurveMenu.ShowAsContext();

                    evt.StopPropagation();
                }
            });
        }

        public void AddKeyframeAtTime(float t)
        {
            (curveProperty.boxedValue as ArrayTimelineCurve<T>).AddKeyframeToCurve(t);
            Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
            RegenerateElement();
        }

        public void AddKeyframeAtTime(float t, int i)
        {
            (curveProperty.boxedValue as ArrayTimelineCurve<T>).AddKeyframeToCurve(t, i);
            Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
            RegenerateElement();
        }

        public void AddKeyframeAtTime(float t, int i, T value)
        {
            (curveProperty.boxedValue as ArrayTimelineCurve<T>).AddKeyframeToCurve(t, i, value);
            Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
            RegenerateElement();
        }
        public void SetKeyframeAtTime(float t, int i, T value)
        {
            (curveProperty.boxedValue as ArrayTimelineCurve<T>).SetKeyframeAtTime(t, i, value);
            Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
            RegenerateElement();
        }

        public void RepresentValue(float t)
        {
            for(int i = 0; i < playbackProperty.arraySize; i++)
            {
                playbackProperty.GetArrayElementAtIndex(i).boxedValue = (curveProperty.boxedValue as ArrayTimelineCurve<T>).EvaluateValue(t)[i];
            }
            playbackProperty.serializedObject.ApplyModifiedProperties();
            playbackProperty.serializedObject.Update();
            //Helper.ApplyChangesToObject();
            MarkDirtyRepaint();
        }

        protected void ResizeLabel()
        {
            try
            {
                w = WrapModeField.Children().ToArray()[0];
                w.style.flexDirection = FlexDirection.Column;
                w.style.flexWrap = Wrap.Wrap;
                Label l2 = w.Q<Label>();
                l2.style.maxWidth = 120;
                l2.style.minWidth = 50;
                l2.style.flexGrow = -1;
                l2.style.flexShrink = -1;
                VisualElement v2 = w.Children().ToArray()[1];
                v2.style.flexGrow = 1;
                v2.style.minHeight = 17;
                v2.style.minWidth = 90;
                v2.style.maxWidth = 120;
            }
            catch
            {
                //lmao
            }
        }
    }
}
#endif