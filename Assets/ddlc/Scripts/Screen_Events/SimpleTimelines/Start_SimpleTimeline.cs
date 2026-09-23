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

        public override void DoEvent()
        {
            timelineToRunThrough.PlayTimeline(debug);
            if (timelineToRunThrough.Timeline.Loop) SimpleTimeline_AnimationSaveLoader.AddAnimation(timelineToRunThrough);
            if (endWithScreen) Sequence_Manager.ProgressScreenEvent += PanicStop;
        }

        void PanicStop()
        {
            timelineToRunThrough.StopTimeline();
            if (SimpleTimeline_AnimationSaveLoader.ContainsAnimation(timelineToRunThrough))
            {
                SimpleTimeline_AnimationSaveLoader.RemoveAnimation(timelineToRunThrough);
            }
        }

        public override string ToString()
        {
            return "DDLC/Play Simple Timeline";
        }
    }
}
