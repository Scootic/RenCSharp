using System.Collections.Generic;
using System;
using UnityEngine;
using UObject = UnityEngine.Object;
using System.Threading;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// AnimationClip-style struct to handle animating things based on data that isn't in-scene (but also some in-scene things, too).
    /// </summary>
    [Serializable]
    public struct SimpleTimeline
    {
        public bool Loop;
        public float Duration;

        private GameObject sceneObject;
        /// <summary>
        /// Should only be used by the SimpleTimelineComponent, or another Scene-based script.
        /// </summary>
        public GameObject SetSceneObject 
        { 
            set 
            { 
                sceneObject = value; 
                foreach(TimelineCurve<object, UObject> curve in Curves)
                {
                    curve.SetRootObject = sceneObject;
                }
            } 
        }
        public readonly bool HasSceneObject => sceneObject != null;

        /// <summary>
        /// Seconds per frame. Ie. "I want 60FPS! Do: 1f / 60f."
        /// </summary>
        public const float SPF = 1f / 60f;
        //figure out how to add timelinecurves of specific types and actually be able to interpret that?
        //object.ToString?
        public List<TimelineCurve<object,UObject>> Curves;

        public readonly async Awaitable RunThroughTimeline(CancellationToken ct)
        {
            float secondsElapsed = 0;

            while(secondsElapsed < Duration || Loop)
            {
                if (ct.IsCancellationRequested) break;
                await Awaitable.WaitForSecondsAsync(SPF);
                secondsElapsed += SPF;

                foreach(TimelineCurve<object, UObject> curve in Curves)
                {
                    curve.Evaluate(secondsElapsed);
                }

                if(Loop && secondsElapsed >= Duration)
                {
                    secondsElapsed = 0;
                }
            }
        }

        public readonly void TimelineResult()
        {
            foreach(TimelineCurve<object, UObject> curve in Curves)
            {
                curve.Evaluate(Duration);
            }
        }
    }
}
