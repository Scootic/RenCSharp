#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UObject = UnityEngine.Object;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    /// <summary>
    /// Base SimpleTimelineField class. Only accessible as a UxmlElement for debugging inside of the UI builder; contains
    /// no TimelineCurve types that can be added to the SimpleTimeline value.
    /// </summary>
    [UxmlElement]
    public partial class SimpleTimelineUITKField : BaseField<SimpleTimeline>
    {
        protected readonly VisualElement SimpleTimelineInfoHolder, TimelineHolder, 
            TimelineControlsHolder, KeyframeControlsHolder, CurrentTimePreview;
        protected readonly TimelineRuler TimelineRuler;
        protected readonly IntegerField CurrentFrameField;

        protected readonly FloatField DurationField, CurrentSecondsField;
        protected readonly Toggle LoopField;

        protected readonly Button BackFrame, PlayPause, ForwardFrame;
        protected static Texture2D bckIco = null, fwdIco = null, playIco = null, pausIco = null, fullRulerLength = null;

        protected SimpleTimeline thisTimeline = new();

        /// <summary>
        /// The scrollable area
        /// </summary>
        protected readonly ScrollView TimelineScrollView;

        /// <summary>
        /// Keyframe Knob property viewer.
        /// </summary>
        protected readonly PropertyField CurrentKeyframeField;
        //? the guys that'll be displayed in the scroll view?
        protected readonly List<TimelineCurveField<object,UObject>>TimelineCurveFields = new();
        /// <summary>
        /// Right-click inside TimelineScrollView.
        /// </summary>
        protected GenericMenu AddNewCurveMenu;
        /// <summary>
        /// Is the timeline currently playing in the editor? (Is the red line moving???)
        /// </summary>
        protected bool playing = false;

        protected float curT = 0;
        /// <summary>
        /// Please override with the types of TimelineCurves you want to be usable in your field.
        /// </summary>
        /// <returns></returns>
        protected virtual Type[] GetValidCurveTypes() { return null; }
        
        public SimpleTimelineUITKField() : this(null) { }

        public SimpleTimelineUITKField(string labelText) : base(labelText, new VisualElement())
        {
            playing = false;
            curT = 0;
            SimpleTimelineUITK_Helper.ReceiveKeyframe = null;
            SimpleTimelineUITK_Helper.ReceiveKeyframe += DisplayKeyframeInformation;
            style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            style.flexDirection = FlexDirection.Column;

            GrabIcons();
            RemoveAt(0); //?
            style.position = Position.Absolute;
            style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            style.bottom = 0;
            style.right = 0;
            style.left = 0;
            style.top = 0;
            name = "SimpleTimelineField";

            #region TimelineInfoHolder
            SimpleTimelineInfoHolder = new() { name = "SimpleTimelineInfoHolder" };
            SimpleTimelineInfoHolder.style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBG;
            SimpleTimelineInfoHolder.style.borderBottomWidth = 1;
            SimpleTimelineInfoHolder.style.borderBottomColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightWidth = 1;
            SimpleTimelineInfoHolder.style.minWidth = 0;
            SimpleTimelineInfoHolder.style.minHeight = 0;
            SimpleTimelineInfoHolder.style.maxHeight = 80;
            SimpleTimelineInfoHolder.style.maxWidth = 235;
            Add(SimpleTimelineInfoHolder);

            DurationField = new("Duration:") { name = "TimelineDuration" };
            DurationField.style.color = Color.white;
            DurationField.style.width = 225;
            DurationField.value = thisTimeline.Duration;
            DurationField.RegisterValueChangedCallback(evt =>
            {
                thisTimeline.Duration = evt.newValue;
                UpdateTimelineScrollSizeBasedOnDuration(evt.newValue);
            });
            SimpleTimelineInfoHolder.Add(DurationField);

            LoopField = new("Loop:") { name = "TimelineLoop" };
            LoopField.style.color = Color.white;
            LoopField.style.width = 225;
            LoopField.value = thisTimeline.Loop;
            LoopField.RegisterValueChangedCallback(evt => { thisTimeline.Loop = evt.newValue; });
            SimpleTimelineInfoHolder.Add(LoopField);
            #endregion

            KeyframeControlsHolder = new() { name = "KeyframeControls" };
            KeyframeControlsHolder.style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBG;
            KeyframeControlsHolder.style.borderRightColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            KeyframeControlsHolder.style.borderRightWidth = 1;
            KeyframeControlsHolder.style.borderTopWidth = 1;
            KeyframeControlsHolder.style.borderTopColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            KeyframeControlsHolder.style.minWidth = 0;
            KeyframeControlsHolder.style.bottom = 0;
            KeyframeControlsHolder.style.flexGrow = 1;
            KeyframeControlsHolder.style.maxWidth = 235;
            KeyframeControlsHolder.style.minHeight = 100;
            Add(KeyframeControlsHolder);

            CurrentKeyframeField = new() { name = "CurrentKeyframe" };
            KeyframeControlsHolder.Add(CurrentKeyframeField);

            #region TimelineHolder
            //holds all timeline chicanery
            TimelineHolder = new() { name = "TimelineHolder" };
            TimelineHolder.style.backgroundColor = style.backgroundColor;
            TimelineHolder.style.position = Position.Absolute;
            TimelineHolder.style.minHeight = 235;
            TimelineHolder.style.minWidth = 500;
            TimelineHolder.style.left = 235;
            TimelineHolder.style.top = 0;
            TimelineHolder.style.bottom = 0;
            TimelineHolder.style.right = 0;
            TimelineHolder.style.maxWidth = 9999;
            TimelineHolder.style.maxHeight = 9999;
            TimelineHolder.style.flexDirection = FlexDirection.Column;
            Add(TimelineHolder);

            #region TimelineControlsHolder
            //holds the play/pause button and stuff!
            TimelineControlsHolder = new() { name = "TimelineControlsHolder" };
            TimelineControlsHolder.style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBG;
            TimelineControlsHolder.style.borderRightColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderLeftColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderBottomColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderRightWidth = 1;
            TimelineControlsHolder.style.borderLeftWidth = 1;
            TimelineControlsHolder.style.borderBottomWidth = 1;
            TimelineControlsHolder.style.flexDirection = FlexDirection.Row;
            TimelineControlsHolder.style.minHeight = 50;
            TimelineControlsHolder.style.minWidth = 500;
            TimelineHolder.Add(TimelineControlsHolder);

            BackFrame = new(() =>
            {
                if (!playing) CurrentFrameField.value = CurrentFrameField.value - 1;
            })
            { name = "GoBackAFrame" };
            BackFrame.iconImage = bckIco;
            Image bf = BackFrame.Q<Image>();
            bf.scaleMode = ScaleMode.ScaleToFit;
            bf.style.height = 45;
            bf.style.width = 45;
            BackFrame.style.height = 50;
            BackFrame.style.width = 50;
            TimelineControlsHolder.Add(BackFrame);

            PlayPause = new(() =>
            {
                playing = !playing;
                PlayPause.iconImage = playing ? pausIco : playIco;
            })
            { name = "Play/Pause"};
            PlayPause.iconImage = playIco;
            Image pp = PlayPause.Q<Image>();
            pp.scaleMode = ScaleMode.ScaleToFit;
            pp.style.height = 45;
            pp.style.width = 45;
            PlayPause.style.height = 50;
            PlayPause.style.width = 50;
            TimelineControlsHolder.Add(PlayPause);

            ForwardFrame = new(() => { if (!playing) CurrentFrameField.value = CurrentFrameField.value + 1; }) { name = "GoForwardAFrame" };
            ForwardFrame.iconImage = fwdIco;
            Image ff = ForwardFrame.Q<Image>();
            ff.scaleMode = ScaleMode.ScaleToFit;
            ff.style.height = 45;
            ff.style.width = 45;
            ForwardFrame.style.height = 50;
            ForwardFrame.style.width = 50;
            TimelineControlsHolder.Add(ForwardFrame);

            CurrentFrameField = new("Frame:") { name = "CurrentFrameField" };
            CurrentFrameField.style.height = 50;
            CurrentFrameField.focusable = false;
            CurrentFrameField.value = 0;
            CurrentFrameField.isReadOnly = true;
            CurrentFrameField.style.color = Color.white;
            Label l1 = CurrentFrameField.Q<Label>();
            l1.style.width = 50;
            l1.style.minWidth = 50;
            l1.style.unityTextAlign = TextAnchor.MiddleLeft;
            VisualElement v1 = CurrentFrameField.Q<VisualElement>("unity-text-input");
            v1.style.minWidth = 70;
            v1.style.minHeight = 30;
            v1.style.alignSelf = Align.Center;
            CurrentFrameField.RegisterValueChangedCallback(evt =>
            {
                curT = (float)evt.newValue / 60f;
                CurrentSecondsField.value = curT;
                //divide frame by 60, then multiply by PixelWidthPerSeconds?
                CurrentTimePreview.style.left = curT * (float)SimpleTimelineUITK_Helper.PixelWidthPerSeconds + 150f;
            });
            TimelineControlsHolder.Add(CurrentFrameField);

            CurrentSecondsField = new("Seconds:") { name = "CurrentSecondsLabel" };
            CurrentSecondsField.style.height = 50;
            CurrentSecondsField.focusable = false;
            CurrentSecondsField.value = 0;
            CurrentSecondsField.isReadOnly = true;
            Label l2 = CurrentSecondsField.Q<Label>();
            l2.style.width = 70;
            l2.style.minWidth = 70;
            l2.style.unityTextAlign = TextAnchor.MiddleLeft;
            VisualElement v2 = CurrentSecondsField.Q<VisualElement>("unity-text-input");
            v2.style.minWidth = 70;
            v2.style.minHeight = 30;
            v2.style.alignSelf = Align.Center;
            CurrentSecondsField.style.color = Color.white;
            TimelineControlsHolder.Add(CurrentSecondsField);
            #endregion

            CreateAddNewCurveMenu();
            TimelineScrollView = new() { name = "TimelineScrollView" };
            TimelineScrollView.style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            TimelineScrollView.style.minHeight = 235;
            TimelineScrollView.style.maxHeight = 9999;
            TimelineScrollView.style.minWidth = 500;
            TimelineScrollView.style.maxWidth = 9999;
            TimelineScrollView.style.bottom = 0;
            TimelineScrollView.style.right = 0;
            TimelineScrollView.style.flexDirection = FlexDirection.Column;
            TimelineScrollView.style.flexGrow = 1;
            TimelineScrollView.style.backgroundImage = fullRulerLength;
            TimelineScrollView.style.unityBackgroundImageTintColor = SimpleTimelineUITK_Helper.HalfTransparentWhite;
            TimelineScrollView.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left,150f));
            TimelineScrollView.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            TimelineScrollView.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32,32));
            TimelineScrollView.RegisterCallback<PointerDownEvent>(evt =>
            {
                if(evt.button == 1) //if right click
                {
                    AddNewCurveMenu.ShowAsContext();
                }
            });
            TimelineHolder.Add(TimelineScrollView);

            TimelineRuler = new() { name = "TimelineRuler" };
            TimelineRuler.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 0 && !playing) //do the red ruler drag that sets cur time!
                {
                    //x seconds = localPos.x / PixelWidthPerSeconds. multiply by 60 for frame? floor???
                    CurrentFrameField.value = Mathf.FloorToInt(evt.localPosition.x /
                        (float)SimpleTimelineUITK_Helper.PixelWidthPerSeconds * 60f);
                }
            });
            TimelineScrollView.Add(TimelineRuler);

            CurrentTimePreview = new() { name = "TimePreview" };
            CurrentTimePreview.style.backgroundColor = Color.red;
            CurrentTimePreview.style.width = 1f;
            CurrentTimePreview.style.maxWidth = 1f;
            CurrentTimePreview.style.minHeight = 500;
            CurrentTimePreview.style.top = 0;
            CurrentTimePreview.style.bottom = 0;
            CurrentTimePreview.style.position = Position.Absolute;
            CurrentTimePreview.style.left = 150f;//this is the boy that gets adjusted when draggening?
            CurrentTimePreview.focusable = false;
            CurrentTimePreview.pickingMode = PickingMode.Ignore;
            TimelineScrollView.Add(CurrentTimePreview);

            #endregion
            GenerateTimelineCurveFields();

            schedule.Execute(PreviewTimelineUpdate).Every(16).StartingIn(0);
        }

        public SimpleTimelineUITKField(string labelText, SimpleTimeline st) : base(labelText, new VisualElement())
        {
            thisTimeline = st;

            playing = false;
            curT = 0;
            SimpleTimelineUITK_Helper.ReceiveKeyframe = null;
            SimpleTimelineUITK_Helper.ReceiveKeyframe += DisplayKeyframeInformation;
            style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            style.flexDirection = FlexDirection.Column;

            GrabIcons();
            RemoveAt(0); //?
            style.position = Position.Absolute;
            style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            style.bottom = 0;
            style.right = 0;
            style.left = 0;
            style.top = 0;
            name = "SimpleTimelineField";

            #region TimelineInfoHolder
            SimpleTimelineInfoHolder = new() { name = "SimpleTimelineInfoHolder" };
            SimpleTimelineInfoHolder.style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBG;
            SimpleTimelineInfoHolder.style.borderBottomWidth = 1;
            SimpleTimelineInfoHolder.style.borderBottomColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightWidth = 1;
            SimpleTimelineInfoHolder.style.minWidth = 0;
            SimpleTimelineInfoHolder.style.minHeight = 0;
            SimpleTimelineInfoHolder.style.maxHeight = 80;
            SimpleTimelineInfoHolder.style.maxWidth = 235;
            Add(SimpleTimelineInfoHolder);

            DurationField = new("Duration:") { name = "TimelineDuration" };
            DurationField.style.color = Color.white;
            DurationField.style.width = 225;
            DurationField.value = thisTimeline.Duration;
            DurationField.RegisterValueChangedCallback(evt =>
            {
                thisTimeline.Duration = evt.newValue;
                UpdateTimelineScrollSizeBasedOnDuration(evt.newValue);
            });
            SimpleTimelineInfoHolder.Add(DurationField);

            LoopField = new("Loop:") { name = "TimelineLoop" };
            LoopField.style.color = Color.white;
            LoopField.style.width = 225;
            LoopField.value = thisTimeline.Loop;
            LoopField.RegisterValueChangedCallback(evt => { thisTimeline.Loop = evt.newValue; });
            SimpleTimelineInfoHolder.Add(LoopField);
            #endregion

            KeyframeControlsHolder = new() { name = "KeyframeControls" };
            KeyframeControlsHolder.style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBG;
            KeyframeControlsHolder.style.borderRightColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            KeyframeControlsHolder.style.borderRightWidth = 1;
            KeyframeControlsHolder.style.borderTopWidth = 1;
            KeyframeControlsHolder.style.borderTopColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            KeyframeControlsHolder.style.minWidth = 0;
            KeyframeControlsHolder.style.bottom = 0;
            KeyframeControlsHolder.style.flexGrow = 1;
            KeyframeControlsHolder.style.maxWidth = 235;
            KeyframeControlsHolder.style.minHeight = 100;
            Add(KeyframeControlsHolder);

            CurrentKeyframeField = new() { name = "CurrentKeyframe" };
            KeyframeControlsHolder.Add(CurrentKeyframeField);

            #region TimelineHolder
            //holds all timeline chicanery
            TimelineHolder = new() { name = "TimelineHolder" };
            TimelineHolder.style.backgroundColor = style.backgroundColor;
            TimelineHolder.style.position = Position.Absolute;
            TimelineHolder.style.minHeight = 235;
            TimelineHolder.style.minWidth = 500;
            TimelineHolder.style.left = 235;
            TimelineHolder.style.top = 0;
            TimelineHolder.style.bottom = 0;
            TimelineHolder.style.right = 0;
            TimelineHolder.style.maxWidth = 9999;
            TimelineHolder.style.maxHeight = 9999;
            TimelineHolder.style.flexDirection = FlexDirection.Column;
            Add(TimelineHolder);

            #region TimelineControlsHolder
            //holds the play/pause button and stuff!
            TimelineControlsHolder = new() { name = "TimelineControlsHolder" };
            TimelineControlsHolder.style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBG;
            TimelineControlsHolder.style.borderRightColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderLeftColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderBottomColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            TimelineControlsHolder.style.borderRightWidth = 1;
            TimelineControlsHolder.style.borderLeftWidth = 1;
            TimelineControlsHolder.style.borderBottomWidth = 1;
            TimelineControlsHolder.style.flexDirection = FlexDirection.Row;
            TimelineControlsHolder.style.minHeight = 50;
            TimelineControlsHolder.style.minWidth = 500;
            TimelineHolder.Add(TimelineControlsHolder);

            BackFrame = new(() =>
            {
                if (!playing) CurrentFrameField.value = CurrentFrameField.value - 1;
            })
            { name = "GoBackAFrame" };
            BackFrame.iconImage = bckIco;
            Image bf = BackFrame.Q<Image>();
            bf.scaleMode = ScaleMode.ScaleToFit;
            bf.style.height = 45;
            bf.style.width = 45;
            BackFrame.style.height = 50;
            BackFrame.style.width = 50;
            TimelineControlsHolder.Add(BackFrame);

            PlayPause = new(() =>
            {
                playing = !playing;
                PlayPause.iconImage = playing ? pausIco : playIco;
            })
            { name = "Play/Pause" };
            PlayPause.iconImage = playIco;
            Image pp = PlayPause.Q<Image>();
            pp.scaleMode = ScaleMode.ScaleToFit;
            pp.style.height = 45;
            pp.style.width = 45;
            PlayPause.style.height = 50;
            PlayPause.style.width = 50;
            TimelineControlsHolder.Add(PlayPause);

            ForwardFrame = new(() => { if (!playing) CurrentFrameField.value = CurrentFrameField.value + 1; }) { name = "GoForwardAFrame" };
            ForwardFrame.iconImage = fwdIco;
            Image ff = ForwardFrame.Q<Image>();
            ff.scaleMode = ScaleMode.ScaleToFit;
            ff.style.height = 45;
            ff.style.width = 45;
            ForwardFrame.style.height = 50;
            ForwardFrame.style.width = 50;
            TimelineControlsHolder.Add(ForwardFrame);

            CurrentFrameField = new("Frame:") { name = "CurrentFrameField" };
            CurrentFrameField.style.height = 50;
            CurrentFrameField.focusable = false;
            CurrentFrameField.value = 0;
            CurrentFrameField.isReadOnly = true;
            CurrentFrameField.style.color = Color.white;
            Label l1 = CurrentFrameField.Q<Label>();
            l1.style.width = 50;
            l1.style.minWidth = 50;
            l1.style.unityTextAlign = TextAnchor.MiddleLeft;
            VisualElement v1 = CurrentFrameField.Q<VisualElement>("unity-text-input");
            v1.style.minWidth = 70;
            v1.style.minHeight = 30;
            v1.style.alignSelf = Align.Center;
            CurrentFrameField.RegisterValueChangedCallback(evt =>
            {
                curT = (float)evt.newValue / 60f;
                CurrentSecondsField.value = curT;
                //divide frame by 60, then multiply by PixelWidthPerSeconds?
                CurrentTimePreview.style.left = curT * (float)SimpleTimelineUITK_Helper.PixelWidthPerSeconds + 150f;
            });
            TimelineControlsHolder.Add(CurrentFrameField);

            CurrentSecondsField = new("Seconds:") { name = "CurrentSecondsLabel" };
            CurrentSecondsField.style.height = 50;
            CurrentSecondsField.focusable = false;
            CurrentSecondsField.value = 0;
            CurrentSecondsField.isReadOnly = true;
            Label l2 = CurrentSecondsField.Q<Label>();
            l2.style.width = 70;
            l2.style.minWidth = 70;
            l2.style.unityTextAlign = TextAnchor.MiddleLeft;
            VisualElement v2 = CurrentSecondsField.Q<VisualElement>("unity-text-input");
            v2.style.minWidth = 70;
            v2.style.minHeight = 30;
            v2.style.alignSelf = Align.Center;
            CurrentSecondsField.style.color = Color.white;
            TimelineControlsHolder.Add(CurrentSecondsField);
            #endregion

            CreateAddNewCurveMenu();
            TimelineScrollView = new() { name = "TimelineScrollView" };
            TimelineScrollView.style.backgroundColor = SimpleTimelineUITK_Helper.SecondLayerBorder;
            TimelineScrollView.style.minHeight = 235;
            TimelineScrollView.style.maxHeight = 9999;
            TimelineScrollView.style.minWidth = 500;
            TimelineScrollView.style.maxWidth = 9999;
            TimelineScrollView.style.bottom = 0;
            TimelineScrollView.style.right = 0;
            TimelineScrollView.style.flexDirection = FlexDirection.Column;
            TimelineScrollView.style.flexGrow = 1;
            TimelineScrollView.style.backgroundImage = fullRulerLength;
            TimelineScrollView.style.unityBackgroundImageTintColor = SimpleTimelineUITK_Helper.HalfTransparentWhite;
            TimelineScrollView.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, 150f));
            TimelineScrollView.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
            TimelineScrollView.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(32, 32));
            TimelineScrollView.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1) //if right click
                {
                    AddNewCurveMenu.ShowAsContext();
                }
            });
            TimelineHolder.Add(TimelineScrollView);

            TimelineRuler = new() { name = "TimelineRuler" };
            TimelineRuler.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 0 && !playing) //do the red ruler drag that sets cur time!
                {
                    //x seconds = localPos.x / PixelWidthPerSeconds. multiply by 60 for frame? floor???
                    CurrentFrameField.value = Mathf.FloorToInt(evt.localPosition.x /
                        (float)SimpleTimelineUITK_Helper.PixelWidthPerSeconds * 60f);
                }
            });
            TimelineScrollView.Add(TimelineRuler);

            CurrentTimePreview = new() { name = "TimePreview" };
            CurrentTimePreview.style.backgroundColor = Color.red;
            CurrentTimePreview.style.width = 1f;
            CurrentTimePreview.style.maxWidth = 1f;
            CurrentTimePreview.style.minHeight = 500;
            CurrentTimePreview.style.top = 0;
            CurrentTimePreview.style.bottom = 0;
            CurrentTimePreview.style.position = Position.Absolute;
            CurrentTimePreview.style.left = 150f;//this is the boy that gets adjusted when draggening?
            CurrentTimePreview.focusable = false;
            CurrentTimePreview.pickingMode = PickingMode.Ignore;
            TimelineScrollView.Add(CurrentTimePreview);

            #endregion
            GenerateTimelineCurveFields();

            schedule.Execute(PreviewTimelineUpdate).Every(16).StartingIn(0);
        }

        protected void UpdateTimelineScrollSizeBasedOnDuration(float newDurInSeconds)
        {
            //reset cur frame field
            CurrentFrameField.value = 0;
            CurrentSecondsField.value = 0;
            //set guide position back to 0

            TimelineScrollView.style.width = newDurInSeconds * (float)SimpleTimelineUITK_Helper.PixelWidthPerSeconds;
        }

        public void AddNewTimelineCurve(TimelineCurve<object, UObject> newCurve) 
        {
            thisTimeline.Curves.Add(newCurve);
            GenerateTimelineCurveFields();
        }

        public void RemoveTimelineCurve(TimelineCurve<object, UObject> toRemove)
        {
            thisTimeline.Curves.Remove(toRemove);
            GenerateTimelineCurveFields();
        }

        protected void GenerateTimelineCurveFields()
        {
            foreach(TimelineCurveField<object,UObject> curveField in TimelineCurveFields)
            {
                TimelineScrollView.Remove(curveField);
            }
            TimelineCurveFields.Clear();
            if (thisTimeline.Curves == null) return;
            for(int i = 0; i < thisTimeline.Curves.Count; i++)
            {
                TimelineCurveField<object, UObject> t = new($"Curve {i + 1}", thisTimeline.Curves[i]);
                t.DeleteMeAction += delegate { RemoveTimelineCurve(t.value); };
                TimelineScrollView.Add(t); //adding to the timeline scrollview should place it in the content section? i hope?
            }
            CurrentTimePreview.BringToFront();
        }

        protected void DisplayKeyframeInformation(TimelineKeyframe<object> keyframeToDisplay)
        {
            CurrentKeyframeField.Unbind();
            CurrentKeyframeField.BindProperty(new SerializedObject(keyframeToDisplay));
        }

        protected void CreateAddNewCurveMenu()
        {
            AddNewCurveMenu = new();
            if (GetValidCurveTypes() == null) return;

            foreach (Type t in GetValidCurveTypes())
            {
                //this mans should always be a stinkin' TimelineCurve. (God I hope...)
                object c = Activator.CreateInstance(t);
                AddNewCurveMenu.AddItem(new GUIContent($"Add New {c.ToString()}"), false, delegate
                {
                    AddNewTimelineCurve(c as TimelineCurve<object, UObject>);
                });
            }
        }

        protected void GrabIcons()
        {
            if(bckIco == null) bckIco = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UITK_Simple_Timeline/UITK_SimpleTimeline_Icons/bckicon.png");
            if(!bckIco) Debug.LogError("SimpleTimelineUITKField.cs can't find bckicon.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
            if(fwdIco == null) fwdIco = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UITK_Simple_Timeline/UITK_SimpleTimeline_Icons/fwdicon.png");
            if(!fwdIco) Debug.LogError("SimpleTimelineUITKField.cs can't find fwdicon.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
            if(playIco == null) playIco = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UITK_Simple_Timeline/UITK_SimpleTimeline_Icons/playicon.png");
            if(!playIco) Debug.LogError("SimpleTimelineUITKField.cs can't find playicon.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
            if(pausIco == null) pausIco = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UITK_Simple_Timeline/UITK_SimpleTimeline_Icons/pauseicon.png");
            if(!pausIco) Debug.LogError("SimpleTimelineUITKField.cs can't find pauseicon.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
            if (fullRulerLength == null) fullRulerLength = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UITK_Simple_Timeline/UITK_SimpleTimeline_Icons/fullrulerlength.png");
            if (!fullRulerLength) Debug.LogError("SimpleTimelineUITKField.cs can't find fullrulerlength.png. Did you move the UITK_Simple_Timeline folder from the root Asset folder?");
        }

        protected void PreviewTimelineUpdate()
        {
            if (!playing) return;
            if (curT >= DurationField.value)
            {
                curT = 0;
                playing = false;
            }
            //count up every frame and do debug.logs?!

            CurrentFrameField.value++;

            foreach(TimelineCurveField<object,UObject> curve in TimelineCurveFields)
            {
                curve.value.EvaluateMessage(curT);
            }
        }
    }
}
#endif