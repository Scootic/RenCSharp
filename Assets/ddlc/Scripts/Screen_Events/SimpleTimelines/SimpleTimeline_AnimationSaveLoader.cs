using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UITK_SimpleTimeline;
using System.Threading;
using System;
namespace RenCSharp.Sequences
{
    public class SimpleTimeline_AnimationSaveLoader : MonoBehaviour
    {
        private static readonly List<SimpleTimelineAsset> activeTimelineAssets = new();
        private static SimpleTimeline_AnimationSaveLoader instance;
        private Action curRedoAnims;
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

        void RedoAnimations(float[] times)
        {
            for(int i = 0; i < activeTimelineAssets.Count; i++)
            {
                activeTimelineAssets[i].PlayTimeline(false, times[i]);
            }

            SaveLoad.SavingDoneEvent -= curRedoAnims;
        }

        void SaveAnimations(SaveData sd)
        {
            float[] times = new float[activeTimelineAssets.Count];
            string[] assetRefs = new string[activeTimelineAssets.Count];
            for (int i = 0; i < activeTimelineAssets.Count; i++)
            {
                activeTimelineAssets[i].StopTimeline();
                activeTimelineAssets[i].Timeline.TimelineInitial(); //essentially undo an animation while we're saving.
                times[i] = activeTimelineAssets[i].Timeline.SecondsElapsed;
                assetRefs[i] = activeTimelineAssets[i].Myself.AssetGUID;
            }
            sd.CustomJSONData ??= new();
            if (sd.CustomJSONData.Count < 1) sd.ScaleCustomJSONToCount(1);
            
            sd.CustomJSONData[0] = JsonUtility.ToJson(assetRefs);
            Debug.Log("Saving SimpleTimeline animations: " + sd.CustomJSONData[0]);
            curRedoAnims = () => { RedoAnimations(times); };
            SaveLoad.SavingDoneEvent += curRedoAnims;   
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
