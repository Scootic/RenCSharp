using UITK_SimpleTimeline;
using UnityEngine;
using System.Threading;
namespace RenCSharp.Sequences
{
    public class Start_SimpleTimeline : Screen_Event
    {
        [SerializeField] private bool endWithScreen;
        [SerializeField] private bool debug = false;
        /// <summary>
        /// Openable through drawer shenanigans!
        /// </summary>
        [SerializeField] private SimpleTimelineAsset timelineToRunThrough;
        private Awaitable activeTimeline;

        public override async void DoEvent()
        {
            activeTimeline = debug ? timelineToRunThrough.Timeline.RunThroughTimelineDebug(new CancellationToken()): 
                timelineToRunThrough.Timeline.RunThroughTimeline(new CancellationToken());
            if (endWithScreen) Script_Manager.ProgressScreenEvent += PanicStop;
            if (timelineToRunThrough.Timeline.Loop) SimpleTimeline_AnimationSaveLoader.AddAnimation(timelineToRunThrough.Timeline);
            await activeTimeline; //start it?
        }

        void PanicStop()
        {
            activeTimeline.Cancel();
            timelineToRunThrough.Timeline.TimelineResult();
            if (SimpleTimeline_AnimationSaveLoader.ContainsAnimation(timelineToRunThrough.Timeline))
            {
                SimpleTimeline_AnimationSaveLoader.RemoveAnimation(timelineToRunThrough.Timeline);
            }
        }

        public override string ToString()
        {
            return "DDLC/Play Simple Timeline";
        }
    }
}
