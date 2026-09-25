using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Only exists as a way to store SimpleTimelines. You don't necessarily need one of these; if you want to store
    /// SimpleTimelines in some other type of ScriptableObject, you'd just need to add an accessible SimpleTimeline to it.
    /// (Either public or [SerializeField].)<br/><br/>The SimpleTimeline PropertyDrawer should give you the button you need
    /// to open the SimpleTimeline_EditorWindow.
    /// </summary>
    [CreateAssetMenu(menuName = "UITK_SimpleTimeline/SimpleTimeline Asset")]
    public class SimpleTimelineAsset : ScriptableObject
    {
        /// <summary>
        /// Public AssetRef in case you need to load SimpleTimelineAssets in a more complex manner.
        /// </summary>
        public AssetReference Myself => myself;
        [SerializeField, Tooltip("Should be set automatically during asset creation.")] private AssetReference myself = null;
        public SimpleTimeline Timeline = new(5);
        
        private Awaitable activeTimeline;
        private CancellationTokenSource myCTS = new();

        public SimpleTimelineAsset(SimpleTimeline timeline)
        {
            Timeline = timeline;
        }

        public Awaitable SetActiveTimeline { set { activeTimeline = value; } }

        public async void PlayTimeline(bool debug = false, float initTime = 0)
        {
            myCTS ??= new();
            activeTimeline = debug ? Timeline.RunThroughTimelineDebug(myCTS.Token, initTime) : Timeline.RunThroughTimeline(myCTS.Token, initTime);
            await activeTimeline;
        }

        public void StopTimeline()
        {
            myCTS.Cancel();
            myCTS = new(); //?
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(myself.AssetGUID) && !Application.isPlaying && !(UnityEditor.EditorApplication.isCompiling || UnityEditor.EditorApplication.isUpdating))
            {
                myself = this.SetObjectAddressable();
            }
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(myself.AssetGUID) && !Application.isPlaying && !(UnityEditor.EditorApplication.isCompiling || UnityEditor.EditorApplication.isUpdating))
            {
                myself = this.SetObjectAddressable();
            }
        }
#endif
    }
}
