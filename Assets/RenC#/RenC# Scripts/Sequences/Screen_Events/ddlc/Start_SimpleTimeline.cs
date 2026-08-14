using UITK_SimpleTimeline;
using UnityEngine;
using System.Threading;
namespace RenCSharp.Sequences
{
    public class Start_SimpleTimeline : Screen_Event
    {
        [SerializeField] private bool endWithScreen;
        /// <summary>
        /// Openable through drawer shenanigans!
        /// </summary>
        [SerializeField] private SimpleTimelineAsset timelineToRunThrough;
        private Awaitable activeTimeline;

        public override async void DoEvent()
        {
            activeTimeline = timelineToRunThrough.timeline.RunThroughTimeline(new CancellationToken());
            if (endWithScreen) Script_Manager.ProgressScreenEvent += PanicStop;
            await activeTimeline; //start it?
        }

        void PanicStop()
        {
            activeTimeline.Cancel();
            timelineToRunThrough.timeline.TimelineResult();
        }

        public override string ToString()
        {
            return "DDLC/Play Simple Timeline";
        }
    }
}
