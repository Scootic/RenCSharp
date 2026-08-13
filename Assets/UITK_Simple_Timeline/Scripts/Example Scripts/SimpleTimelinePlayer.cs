
using UnityEngine;

namespace UITK_SimpleTimeline
{
    public class SimpleTimelinePlayer : MonoBehaviour
    {
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private SimpleTimelineAsset timelineToPlay;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (playOnStart) PlayTimeline();
        }

        public void PlayTimeline()
        {
            _ = timelineToPlay.timeline.RunThroughTimeline();
        }
    }
}
