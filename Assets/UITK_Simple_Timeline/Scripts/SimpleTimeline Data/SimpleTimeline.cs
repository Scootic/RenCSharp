using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// AnimationClip-style struct to handle animating things based on data that isn't in-scene (but also some in-scene things, too).
    /// If you want to reference a SimpleTimeline without using a SimpleTimelineAsset, you'll want to keep track of some sort of
    /// CancellationTokenSources- that's how you'll be able to stop the Awaitable that runs through the timeline data.
    /// </summary>
    [Serializable]
    public struct SimpleTimeline : IDefaultableNotNull<SimpleTimeline>
    {
        public bool Loop;
        [Min(0)] public float Duration, PlaybackSpeed;
        /// <summary>
        /// Should only be used by the SimpleTimelineComponent, or another Scene-based script.
        /// </summary>
        private GameObject sceneObject;
        /// <summary>
        /// Seconds per frame. Ie. "I want 60FPS! Do: 1f / 60f."
        /// </summary>
        public const float SPF = 1f / 60f;
        //figure out how to add timelinecurves of specific types and actually be able to interpret that?
        //object.ToString?
        [SerializeReference] public List<TimelineCurve> Curves;
        /// <summary>
        /// Used to tie an object inside an active scene to all of the TimelineCurves inside of a SimpleTimeline.
        /// Useful if you're trying to animate an in-scene object directly.
        /// </summary>
        public GameObject SetSceneObject
        {
            set
            {
                sceneObject = value;
                foreach (TimelineCurve curve in Curves)
                {
                    curve.SetRootObject(sceneObject);
                }
            }
        }
        public readonly bool HasSceneObject => sceneObject != null;

        private float secondsElapsed;
        public readonly float SecondsElapsed => secondsElapsed;

        public SimpleTimeline(float duration)
        {
            Loop = false;
            Duration = duration;
            PlaybackSpeed = 1;
            sceneObject = null;
            secondsElapsed = 0;
            Curves = new();
        }
        /// <summary>
        /// Copies the curves using SimpleTimelineExtensions' CopyClassListValuesThroughReflection
        /// </summary>
        /// <param name="copy">The SimpleTimeline you want to copy.</param>
        public SimpleTimeline(SimpleTimeline copy)
        {
            Loop = copy.Loop;
            Duration = copy.Duration;
            PlaybackSpeed = copy.PlaybackSpeed;
            secondsElapsed = 0;
            sceneObject = null;
            Curves = SimpleTimelineExtensions.DeepCopyListFromJSON(copy.Curves);
        }
        /// <summary>
        /// Awaitable that copies the curves using SimpleTimelineExtensions' CopyClassListValuesThroughReflection
        /// </summary>
        /// <param name="copy">The SimpleTimeline you want to copy.</param>
        /// <returns>The copy of the given SimpleTimeline.</returns>
        public static async Awaitable<SimpleTimeline> CopySimpleTimeline(SimpleTimeline copy)
        {
            return new()
            {
                Duration = copy.Duration,
                Loop = copy.Loop,
                PlaybackSpeed = copy.PlaybackSpeed,
                sceneObject = null,
                Curves = await SimpleTimelineExtensions.DeepCopyListFromJSONAsync(copy.Curves)
            };

        }

        public readonly SimpleTimeline Default()
        {
            return new() 
            {
                Loop = false,
                Duration = 10,
                Curves = new(),
                PlaybackSpeed = 1f
            };
        }

        /// <summary>
        /// An awaitable that goes through the timeline's curves, Evaluating() them at the seconds elapsed.
        /// Advances every SPF in seconds (by default, SPF is 1/60 to replicate 60fps).
        /// </summary>
        /// <param name="ct">CancellationToken so you can bail out of the timeline whenever you feel like it.</param>
        /// <param name="initialTime">min 0 please</param>
        /// <returns>Diddly squat.</returns>
        public async Awaitable RunThroughTimeline(CancellationToken ct, float initialTime = 0)
        {
            try
            {
                foreach (TimelineCurve tc in Curves)
                {
                    tc.OnPlay();
                }

                secondsElapsed = initialTime -(SPF * PlaybackSpeed); //start the timeline BEFORE 0 so we can evaluate at 0 and not just skip over.

                while (secondsElapsed < Duration || Loop)
                {
                    if (ct.IsCancellationRequested)
                    {
                        TimelineResult();
                        break;
                    }
                    await Awaitable.WaitForSecondsAsync(SPF);
                    secondsElapsed += SPF * PlaybackSpeed;

                    foreach (TimelineCurve curve in Curves)
                    {
                        curve.Evaluate(secondsElapsed);
                    }

                    if (Loop && secondsElapsed >= Duration)
                    {
                        secondsElapsed = -SPF * PlaybackSpeed;
                    }
                }
            }catch
            {
                TimelineResult();
                return;
            }
        }
        /// <summary>
        /// Same as RunThroughTimeline(), but also logs the EvaluateMessage() at seconds elapsed while also
        /// doing the Evaluate() behavior.
        /// </summary>
        /// <param name="ct">CancellationToken so you can bail out of the timeline whenever you feel like it.</param>
        /// <param name="initialTime">min 0 please</param>
        /// <returns>Diddly squat 2.</returns>
        public async Awaitable RunThroughTimelineDebug(CancellationToken ct, float initialTime = 0)
        {
            try
            {
                foreach (TimelineCurve tc in Curves)
                {
                    tc.OnPlay();
                }

                secondsElapsed = initialTime -(SPF * PlaybackSpeed);

                while (secondsElapsed < Duration || Loop)
                {
                    if (ct.IsCancellationRequested)
                    {
                        TimelineResultDebug();
                        break;
                    }
                    await Awaitable.WaitForSecondsAsync(SPF);
                    secondsElapsed += SPF * PlaybackSpeed;

                    string msg = "";

                    foreach (TimelineCurve curve in Curves)
                    {
                        msg += $"\n{curve.EvaluateMessage(secondsElapsed)}";
                        curve.Evaluate(secondsElapsed);
                    }

                    Debug.Log(msg);

                    if (Loop && secondsElapsed >= Duration)
                    {
                        secondsElapsed = -SPF * PlaybackSpeed;
                    }
                }
            }
            catch
            {
                TimelineResultDebug();
                return;
            }
        }
        /// <summary>
        /// Evaluates the end of every curve in the timeline (Evaluate() at Duration).
        /// </summary>
        public readonly void TimelineResult()
        {
            foreach(TimelineCurve curve in Curves)
            { 
                curve.Evaluate(Duration);
            }
        }
        /// <summary>
        /// Same as TimelineResult, but also Debug.Logs the EvaluateMessage() at Duration.
        /// </summary>
        public readonly void TimelineResultDebug()
        {
            string msg = "";
            foreach(TimelineCurve curve in Curves)
            {
                msg += $"\n{curve.EvaluateMessage(Duration)}";
                curve.Evaluate(Duration);
            }
            Debug.Log(msg);
        }
        /// <summary>
        /// Evaluates the beginning of every curve in the timeline (Evaluate() at 0).
        /// </summary>
        public readonly void TimelineInitial()
        {
            foreach(TimelineCurve curve in Curves)
            {
                curve.Evaluate(0);
            }
        }
        /// <summary>
        /// Same as TimelineInitial, but also Debug.Logs the EvaluateMessage() at 0.
        /// </summary>
        public readonly void TimelineInitialDebug()
        {
            string msg = "";
            foreach (TimelineCurve curve in Curves)
            {
                msg += $"\n{curve.EvaluateMessage(0)}";
                curve.Evaluate(0);
            }
            Debug.Log(msg);
        }
    }
}
