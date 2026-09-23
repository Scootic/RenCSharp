using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UITK_SimpleTimeline;
using System.Threading;
namespace RenCSharp.Sequences
{
    public class SimpleTimeline_AnimationSaveLoader : MonoBehaviour
    {
        private static readonly HashSet<SimpleTimelineAsset> activeTimelineAssets = new();
        private static SimpleTimeline_AnimationSaveLoader instance;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }else if(instance != this)
            {
                Destroy(this);
            }
            SaveLoad.SaveCustomData += SaveAnimations;
            SaveLoad.LoadCustomData += LoadAnimations;
        }

        void OnDisable()
        {
            SaveLoad.SaveCustomData -= SaveAnimations;
            SaveLoad.LoadCustomData -= LoadAnimations;
        }

        public static void AddAnimation(SimpleTimelineAsset st)
        {
            Debug.Log("Adding ST to active timelines");
            activeTimelineAssets.Add(st);
        }

        public static void RemoveAnimation(SimpleTimelineAsset st)
        {
            activeTimelineAssets.Remove(st);
        }

        public static bool ContainsAnimation(SimpleTimelineAsset st)
        {
            return activeTimelineAssets.Contains(st);
        }

        void SaveAnimations(SaveData sd)
        {
            sd.CustomJSONData ??= new();
            if (sd.CustomJSONData.Count < 1) sd.ScaleCustomJSONToCount(1);
            string[] assetRefs = new string[activeTimelineAssets.Count];
            int i = 0;
            foreach(SimpleTimelineAsset sta in activeTimelineAssets)
            {
                assetRefs[i] = sta.Myself.AssetGUID;
                i++;
            }
            sd.CustomJSONData[0] = JsonUtility.ToJson(assetRefs);
            Debug.Log("Saving SimpleTimeline animations: " + sd.CustomJSONData[0]);
        }

        async void LoadAnimations(List<string> jasonData)
        {
            foreach(SimpleTimelineAsset sta in activeTimelineAssets)
            {
                sta.StopTimeline();
            }
            activeTimelineAssets.Clear();
            try
            {
                string[] t = JsonUtility.FromJson<string[]>(jasonData[0]);

                foreach (string star in t)
                {
                    SimpleTimelineAsset goober = await Addressables.LoadAssetAsync<SimpleTimelineAsset>(star).Task;
                    activeTimelineAssets.Add(goober);
                    goober.PlayTimeline(false);
                }
            }
            catch
            {
                Debug.Log("No stinkin' jason data to start new animations with :(");
            }
        }
    }
}
