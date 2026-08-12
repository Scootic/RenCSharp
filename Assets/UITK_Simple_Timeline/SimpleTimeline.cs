using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
namespace UITK_SimpleTimeline
{
    [Serializable]
    public struct SimpleTimeline
    {
        public bool Loop;
        public float Duration;

        /// <summary>
        /// Seconds per frame. Ie. "I want 60FPS! Do: 1f / 60f."
        /// </summary>
        public const float SPF = 1f / 60f;

        public List<TimelineCurve<object>> Curves;

        public readonly async Awaitable RunThroughTimeline()
        {
            int frame = 0;
            float secondsElapsed = 0;

            while(secondsElapsed < Duration || Loop)
            {
                await Awaitable.WaitForSecondsAsync(SPF);
                frame++;
                secondsElapsed += SPF;

                foreach(TimelineCurve<object> curve in Curves)
                {
                    curve.Evaluate(secondsElapsed);
                }

                if(Loop && secondsElapsed >= Duration)
                {
                    frame = 0;
                    secondsElapsed = 0;
                }
            }
        }

        public readonly void TimelineResult()
        {
            foreach(TimelineCurve<object> curve in Curves)
            {
                curve.Evaluate(Duration);
            }
        }
    }
}
