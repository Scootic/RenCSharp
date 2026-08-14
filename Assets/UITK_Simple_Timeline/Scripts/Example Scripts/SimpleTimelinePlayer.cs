
using UnityEngine;
using System.Threading;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Simple timeline player just to play a SimpleTimeline in a scene. PlayTimeline() is a public void
    /// so it can be called by a UnityEvent or other scripts.
    /// </summary>
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
            _ = timelineToPlay.timeline.RunThroughTimeline(new CancellationToken());
        }
    }
}
