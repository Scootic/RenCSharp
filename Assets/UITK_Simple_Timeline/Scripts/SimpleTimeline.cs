using System.Collections.Generic;
using System;
using UnityEngine;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Animation-style struct to handle animating things based on data that isn't in-scene.
    /// </summary>
    [Serializable]
    public struct SimpleTimeline
    {
        public bool Loop;
        public float Duration;

        /// <summary>
        /// Seconds per frame. Ie. "I want 60FPS! Do: 1f / 60f."
        /// </summary>
        public const float SPF = 1f / 60f;

        public List<TimelineCurve<object, object>> Curves;

        public readonly async Awaitable RunThroughTimeline()
        {
            float secondsElapsed = 0;

            while(secondsElapsed < Duration || Loop)
            {
                await Awaitable.WaitForSecondsAsync(SPF);
                secondsElapsed += SPF;

                foreach(TimelineCurve<object, object> curve in Curves)
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
            foreach(TimelineCurve<object, object> curve in Curves)
            {
                curve.Evaluate(Duration);
            }
        }
    }
}
