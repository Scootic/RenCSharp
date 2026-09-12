using UnityEngine;
using System.Collections.Generic;
using UITK_SimpleTimeline;
using System.Threading;
namespace RenCSharp.Sequences
{
    public class SimpleTimeline_AnimationSaveLoader : MonoBehaviour
    {
        private static readonly HashSet<SimpleTimeline> activeTimelines = new();
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

        public static void AddAnimation(SimpleTimeline st)
        {
            activeTimelines.Add(st);
        }

        public static void RemoveAnimation(SimpleTimeline st)
        {
            activeTimelines.Remove(st);
        }

        public static bool ContainsAnimation(SimpleTimeline st)
        {
            return activeTimelines.Contains(st);
        }

        void SaveAnimations(SaveData sd)
        {
            if (sd.CustomJSONData == null) sd.CustomJSONData = new();
            if (sd.CustomJSONData.Count < 1) sd.ScaleCustomJSONToCount(1);
            sd.CustomJSONData[0] = JsonUtility.ToJson(activeTimelines);
            Debug.Log("Saving SimpleTimeline animations: " + sd.CustomJSONData[0]);
        }

        void LoadAnimations(List<string> jasonData)
        {
            HashSet<SimpleTimeline> t = JsonUtility.FromJson<HashSet<SimpleTimeline>>(jasonData[0]);
            if(t != null) 
            { 
                foreach(SimpleTimeline st in activeTimelines)
                {
                    st.RunThroughTimeline(new CancellationToken()).Cancel();
                }
                activeTimelines.Clear();
                foreach(SimpleTimeline st in t)
                {
                    _ = st.RunThroughTimeline(new CancellationToken());
                    activeTimelines.Add(st);
                }
            }
            else Debug.LogError("Couldn't load SimpleTimeline Hashset from the stupid save data!");
        }
    }
}
