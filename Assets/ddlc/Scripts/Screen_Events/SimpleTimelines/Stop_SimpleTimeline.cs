using UITK_SimpleTimeline;
using UnityEngine;
using System.Threading;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Should only really use this boy for infinite looping SimpleTimelines that don't end with screens.
    /// </summary>
    public class Stop_SimpleTimeline : Screen_Event
    {
        [SerializeField] private SimpleTimelineAsset timelineToStop;

        public override void DoEvent()
        {   
            if (SimpleTimeline_AnimationSaveLoader.ContainsAnimation(timelineToStop))
            {
                SimpleTimeline_AnimationSaveLoader.RemoveAnimation(timelineToStop);
            }
            timelineToStop.StopTimeline();
        }

        public override string ToString()
        {
            return "DDLC/Stop SimpleTimeline";
        }
    }
}
