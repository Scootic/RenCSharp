using UITK_SimpleTimeline;
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Start_SimpleTimeline : Screen_Event
    {
        [SerializeField] private bool endWithScreen;
        /// <summary>
        /// Openable through drawer shenanigans!
        /// </summary>
        private SimpleTimeline timelineToRunThrough;
        private Awaitable activeTimeline;

        public override async void DoEvent()
        {
            activeTimeline = timelineToRunThrough.RunThroughTimeline();
            if (endWithScreen) Script_Manager.ProgressScreenEvent += PanicStop;
            await activeTimeline; //start it?
        }

        void PanicStop()
        {
            activeTimeline.Cancel();
            timelineToRunThrough.TimelineResult();
        }

        public override string ToString()
        {
            return "DDLC/Start Simple Timeline";
        }
    }
}
