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
        protected readonly VisualElement Container, SimpleTimelineInfoHolder, TimelineHolder, 
            TimelineControlsHolder, KeyframeControlsHolder, CurrentTimePreview;
        protected readonly IntegerField CurrentFrameField;

        protected readonly FloatField DurationField;
        protected readonly Toggle LoopField;

        protected readonly Button BackFrame, PlayPause, ForwardFrame;
        protected static Texture2D bckIco = null, fwdIco = null, playIco = null, pausIco = null;

        protected SimpleTimeline thisTimeline = new();

        //the scrollable area
        protected readonly ScrollView TimelineScrollView;
        //knob property viewer
        protected readonly PropertyField CurrentKeyframeField;
        //? the guys that'll be displayed in the scroll view?
        protected readonly List<TimelineCurveField<object,object>>TimelineCurveFields = new();
        //boy that'll show up when user right clicks inside curvey area?
        protected GenericMenu AddNewCurveMenu;
        /// <summary>
        /// Is the timeline currently playing in the editor? (Is the red line moving???)
        /// </summary>
        protected bool playing = false;
        /// <summary>
        /// Please override with the types of curved knobs you want to be usable in your field.
        /// </summary>
        /// <returns></returns>
        protected virtual Type[] GetValidCurveTypes() { Debug.LogWarning("No valid types of curves assigned."); return null; }
        public static Action<TimelineKeyframe<object>> ReceiveKeyframe;

        public SimpleTimelineUITKField() : this(null) { }

        public SimpleTimelineUITKField(string labelText) : base(labelText, new VisualElement())
        {
            playing = false;
            ReceiveKeyframe = null;
            ReceiveKeyframe += DisplayKeyframeInformation;
            style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            style.flexDirection = FlexDirection.Column;

            Color secondLayerBG = new(0.2f, 0.2f, 0.2f, 1);
            Color secondLayerBorder = new(0.1f, 0.1f, 0.1f, 1);

            GrabIcons();

            Container = new();
            Container.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            Container.style.height = style.height;
            Container.style.width = style.width;
            Container.name = "Container";
            Add(Container);

            #region TimelineInfoHolder
            SimpleTimelineInfoHolder = new();
            SimpleTimelineInfoHolder.name = "SimpleTimelineInfoHolder";
            SimpleTimelineInfoHolder.style.backgroundColor = secondLayerBG;
            SimpleTimelineInfoHolder.style.borderBottomWidth = 1;
            SimpleTimelineInfoHolder.style.borderBottomColor = secondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightColor = secondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightWidth = 1;
            SimpleTimelineInfoHolder.style.minWidth = 0;
            SimpleTimelineInfoHolder.style.minHeight = 0;
            SimpleTimelineInfoHolder.style.maxHeight = 80;
            SimpleTimelineInfoHolder.style.maxWidth = 235;
            Container.Add(SimpleTimelineInfoHolder);

            DurationField = new("Duration:");
            DurationField.name = "TimelineDuration";
            DurationField.style.color = Color.white;
            DurationField.style.width = 225;
            DurationField.value = thisTimeline.Duration;
            DurationField.RegisterValueChangedCallback(evt =>
            {
                thisTimeline.Duration = evt.newValue;
            });
            SimpleTimelineInfoHolder.Add(DurationField);

            LoopField = new("Loop:");
            LoopField.name = "TimelineLoop";
            LoopField.style.color = Color.white;
            LoopField.style.width = 225;
            LoopField.value = thisTimeline.Loop;
            LoopField.RegisterValueChangedCallback(evt => { thisTimeline.Loop = evt.newValue; });
            SimpleTimelineInfoHolder.Add(LoopField);
            #endregion

            KeyframeControlsHolder = new();
            KeyframeControlsHolder.name = "KeyframeControls";
            KeyframeControlsHolder.style.backgroundColor = secondLayerBG;
            KeyframeControlsHolder.style.borderRightColor = secondLayerBorder;
            KeyframeControlsHolder.style.borderRightWidth = 1;
            KeyframeControlsHolder.style.borderTopWidth = 1;
            KeyframeControlsHolder.style.borderTopColor = secondLayerBorder;
            KeyframeControlsHolder.style.minWidth = 0;
            KeyframeControlsHolder.style.maxWidth = 235;
            KeyframeControlsHolder.style.minHeight = 100;
            Container.Add(KeyframeControlsHolder);

            CurrentKeyframeField = new();
            CurrentKeyframeField.name = "CurrentKeyframe";
            KeyframeControlsHolder.Add(CurrentKeyframeField);

            #region TimelineHolder
            //holds all timeline chicanery
            TimelineHolder = new();
            TimelineHolder.style.backgroundColor = style.backgroundColor;
            TimelineHolder.style.position = Position.Absolute;
            TimelineHolder.name = "TimelineHolder";
            TimelineHolder.style.minHeight = 235;
            TimelineHolder.style.minWidth = 500;
            TimelineHolder.style.left = 235;
            TimelineHolder.style.maxWidth = 9999;
            TimelineHolder.style.maxHeight = 9999;
            TimelineHolder.style.flexDirection = FlexDirection.Column;
            Container.Add(TimelineHolder);

            #region TimelineControlsHolder
            //holds the play/pause button and stuff!
            TimelineControlsHolder = new();
            TimelineControlsHolder.name = "TimelineControlsHolder";
            TimelineControlsHolder.style.backgroundColor = secondLayerBG;
            TimelineControlsHolder.style.borderRightColor = secondLayerBorder;
            TimelineControlsHolder.style.borderLeftColor = secondLayerBorder;
            TimelineControlsHolder.style.borderBottomColor = secondLayerBorder;
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
            });
            BackFrame.name = "GoBackAFrame";
            BackFrame.iconImage = bckIco;
            BackFrame.style.height = 50;
            BackFrame.style.width = 50;
            TimelineControlsHolder.Add(BackFrame);

            PlayPause = new(() =>
            {
                playing = !playing;
                PlayPause.iconImage = playing ? pausIco : playIco;
            });
            PlayPause.name = "Play/Pause";
            PlayPause.iconImage = playIco;
            PlayPause.style.height = 50;
            PlayPause.style.width = 50;
            TimelineControlsHolder.Add(PlayPause);

            ForwardFrame = new(() => { if (!playing) CurrentFrameField.value = CurrentFrameField.value + 1; });
            ForwardFrame.name = "GoForwardAFrame";
            ForwardFrame.iconImage = fwdIco;
            ForwardFrame.style.height = 50;
            ForwardFrame.style.width = 50;
            TimelineControlsHolder.Add(ForwardFrame);

            CurrentFrameField = new("Frame:");
            CurrentFrameField.name = "CurrentFrameField";
            CurrentFrameField.style.height = 50;
            CurrentFrameField.focusable = false;
            CurrentFrameField.value = 0;
            CurrentFrameField.isReadOnly = true;
            CurrentFrameField.style.color = Color.white;
            TimelineControlsHolder.Add(CurrentFrameField);
            #endregion

            TimelineScrollView = new();
            TimelineScrollView.style.backgroundColor = secondLayerBorder;
            TimelineScrollView.name = "TimelineScrollView";
            TimelineScrollView.style.minHeight = 235;
            TimelineScrollView.style.maxHeight = 9999;
            TimelineScrollView.style.minWidth = 500;
            TimelineScrollView.style.maxWidth = 9999;
            TimelineScrollView.style.flexDirection = FlexDirection.Column;
            TimelineHolder.Add(TimelineScrollView);

            
            #endregion
            GenerateTimelineCurveFields();

            CurrentFrameField = new();
            CurrentFrameField.value = 0;
        }

        public SimpleTimelineUITKField(string labelText, SimpleTimeline st) : base(labelText, new VisualElement())
        {
            thisTimeline = st;

            playing = false;
            ReceiveKeyframe = null;
            ReceiveKeyframe += DisplayKeyframeInformation;
            style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            style.flexDirection = FlexDirection.Column;

            Color secondLayerBG = new(0.2f, 0.2f, 0.2f, 1);
            Color secondLayerBorder = new(0.1f, 0.1f, 0.1f, 1);

            GrabIcons();

            Container = new();
            Container.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);
            Container.style.height = style.height;
            Container.style.width = style.width;
            Container.name = "Container";
            Add(Container);

            #region TimelineInfoHolder
            SimpleTimelineInfoHolder = new();
            SimpleTimelineInfoHolder.name = "SimpleTimelineInfoHolder";
            SimpleTimelineInfoHolder.style.backgroundColor = secondLayerBG;
            SimpleTimelineInfoHolder.style.borderBottomWidth = 1;
            SimpleTimelineInfoHolder.style.borderBottomColor = secondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightColor = secondLayerBorder;
            SimpleTimelineInfoHolder.style.borderRightWidth = 1;
            SimpleTimelineInfoHolder.style.minWidth = 0;
            SimpleTimelineInfoHolder.style.minHeight = 0;
            SimpleTimelineInfoHolder.style.maxHeight = 80;
            SimpleTimelineInfoHolder.style.maxWidth = 235;
            Container.Add(SimpleTimelineInfoHolder);

            DurationField = new("Duration:");
            DurationField.name = "TimelineDuration";
            DurationField.style.color = Color.black;
            DurationField.style.width = 225;
            DurationField.value = thisTimeline.Duration;
            DurationField.RegisterValueChangedCallback(evt =>
            {
                thisTimeline.Duration = evt.newValue;
            });
            SimpleTimelineInfoHolder.Add(DurationField);

            LoopField = new("Loop:");
            LoopField.name = "TimelineLoop";
            LoopField.style.color = Color.black;
            LoopField.style.width = 225;
            LoopField.value = thisTimeline.Loop;
            LoopField.RegisterValueChangedCallback(evt => { thisTimeline.Loop = evt.newValue; });
            SimpleTimelineInfoHolder.Add(LoopField);
            #endregion

            KeyframeControlsHolder = new();
            KeyframeControlsHolder.name = "KeyframeControls";
            KeyframeControlsHolder.style.backgroundColor = secondLayerBG;
            KeyframeControlsHolder.style.borderRightColor = secondLayerBorder;
            KeyframeControlsHolder.style.borderRightWidth = 1;
            KeyframeControlsHolder.style.borderTopWidth = 1;
            KeyframeControlsHolder.style.borderTopColor = secondLayerBorder;
            KeyframeControlsHolder.style.minWidth = 0;
            KeyframeControlsHolder.style.maxWidth = 235;
            KeyframeControlsHolder.style.minHeight = 100;
            Container.Add(KeyframeControlsHolder);

            CurrentKeyframeField = new();
            CurrentKeyframeField.name = "CurrentKeyframe";
            KeyframeControlsHolder.Add(CurrentKeyframeField);

            #region TimelineHolder
            //holds all timeline chicanery
            TimelineHolder = new();
            TimelineHolder.style.backgroundColor = style.backgroundColor;
            TimelineHolder.style.position = Position.Absolute;
            TimelineHolder.name = "TimelineHolder";
            TimelineHolder.style.minHeight = 235;
            TimelineHolder.style.minWidth = 500;
            TimelineHolder.style.left = 235;
            TimelineHolder.style.maxWidth = 9999;
            TimelineHolder.style.maxHeight = 9999;
            TimelineHolder.style.flexDirection = FlexDirection.Column;
            Container.Add(TimelineHolder);

            #region TimelineControlsHolder
            //holds the play/pause button and stuff!
            TimelineControlsHolder = new();
            TimelineControlsHolder.name = "TimelineControlsHolder";
            TimelineControlsHolder.style.backgroundColor = secondLayerBG;
            TimelineControlsHolder.style.borderRightColor = secondLayerBorder;
            TimelineControlsHolder.style.borderLeftColor = secondLayerBorder;
            TimelineControlsHolder.style.borderBottomColor = secondLayerBorder;
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
            });
            BackFrame.name = "GoBackAFrame";
            BackFrame.iconImage = bckIco;
            BackFrame.style.height = 50;
            BackFrame.style.width = 50;
            TimelineControlsHolder.Add(BackFrame);

            PlayPause = new(() =>
            {
                playing = !playing;
                PlayPause.iconImage = playing ? pausIco : playIco;
            });
            PlayPause.name = "Play/Pause";
            PlayPause.iconImage = playIco;
            PlayPause.style.height = 50;
            PlayPause.style.width = 50;
            TimelineControlsHolder.Add(PlayPause);

            ForwardFrame = new(() => { if (!playing) CurrentFrameField.value = CurrentFrameField.value + 1; });
            ForwardFrame.name = "GoForwardAFrame";
            ForwardFrame.iconImage = fwdIco;
            ForwardFrame.style.height = 50;
            ForwardFrame.style.width = 50;
            TimelineControlsHolder.Add(ForwardFrame);
            #endregion

            TimelineScrollView = new();
            TimelineScrollView.style.backgroundColor = secondLayerBorder;
            TimelineScrollView.name = "TimelineScrollView";
            TimelineScrollView.style.minHeight = 235;
            TimelineScrollView.style.maxHeight = 9999;
            TimelineScrollView.style.minWidth = 500;
            TimelineScrollView.style.maxWidth = 9999;
            TimelineScrollView.style.flexDirection = FlexDirection.Column;
            TimelineHolder.Add(TimelineScrollView);

            #endregion
            GenerateTimelineCurveFields();

            CurrentFrameField = new();
            CurrentFrameField.value = 0;
        }

        public void AddNewTimelineCurve(TimelineCurve<object, object> newCurve) 
        {
            thisTimeline.Curves.Add(newCurve);
            GenerateTimelineCurveFields();
        }

        public void RemoveTimelineCurve(TimelineCurve<object, object> toRemove)
        {
            thisTimeline.Curves.Remove(toRemove);
            GenerateTimelineCurveFields();
        }

        protected void GenerateTimelineCurveFields()
        {
            foreach(TimelineCurveField<object,object> curveField in TimelineCurveFields)
            {
                Container.Remove(curveField);
            }
            TimelineCurveFields.Clear();
            if (thisTimeline.Curves == null) return;
            for(int i = 0; i < thisTimeline.Curves.Count; i++)
            {
                TimelineCurveField<object, object> t = new($"Curve {i + 1}", thisTimeline.Curves[i]);
                t.DeleteMeAction += delegate { RemoveTimelineCurve(t.value); };
                Container.Add(t);
            }
        }

        protected void DisplayKeyframeInformation(TimelineKeyframe<object> keyframeToDisplay)
        {
            CurrentKeyframeField.Unbind();
            CurrentKeyframeField.BindProperty(new SerializedObject(keyframeToDisplay));
        }

        protected void GrabIcons()
        {
            if (bckIco == null) bckIco = Resources.Load<Texture2D>("UITK_SimpleTimeline_Icons/bckicon.png");
            if (fwdIco == null) fwdIco = Resources.Load<Texture2D>("UITK_SimpleTimeline_Icons/fwdicon.png");
            if (playIco == null) playIco = Resources.Load<Texture2D>("UITK_SimpleTimeline_Icons/playicon.png");
            if (pausIco == null) pausIco = Resources.Load<Texture2D>("UITK_SimpleTimeline_Icons/pauseicon.png");
        }

        protected void PreviewTimelineUpdate()
        {
            if (!playing || CurrentFrameField.value * 60f >= DurationField.value) return;
            //count up every frame and do debug.logs?!

            CurrentFrameField.value++;
        }
    }
}
#endif