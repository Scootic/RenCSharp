using UnityEngine;

namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Only exists as a way to store SimpleTimelines. You don't necessarily need one of these.
    /// </summary>
    [CreateAssetMenu(menuName = "SimpleTimeline Asset")]
    public class SimpleTimelineAsset : ScriptableObject
    {
        public SimpleTimeline timeline;

        public SimpleTimelineAsset(SimpleTimeline timeline)
        {
            this.timeline = timeline;
        }
    }
}
