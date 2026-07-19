using System;
using System.Collections.Generic;
using UnityEngine;
namespace RenCSharp.Combat.Enemies
{
    //only created by the EnemyAttackTimelineEditor?
    [CreateAssetMenu(menuName = "New Enemy Attack Timeline")]
    public class EnemyAttackTimelineData : EnemyAttack
    {
        //split dict into lists because EVIL?!?
        [SerializeField, Tooltip("Pretty please don't change this in here, just for debug.")] private List<int> keyframes = new();
        [SerializeField, Tooltip("Pretty please don't change this in here, just for debug.")] private List<ProjectileFrameData> frameData = new();
        [SerializeField] private bool debug = false;
        public void DebugTimelineInformation()
        {
            string s1 = $"All stored projectiles in {name}: ";
            foreach (Base_Projectile bp in projectilesThatSpawn)
            {
                s1 += $"\n{bp.gameObject.name}";
            }
            Debug.Log(s1);
            string s3 = "All timeline information placed into the split lists: ";
            for (int i = 0; i < keyframes.Count; i++)
            {
                s3 += $"\n At Frame {keyframes[i]}, there's: ";
                foreach (ProjectileSnub ps in frameData[i].ProjectilesSpawnedAtFrame)
                {
                    s3 += $"{ps.ToString()}; ";
                }
            }
            Debug.Log(s3);
        }

        private void OnValidate()
        {
            if(debug)DebugTimelineInformation();
        }

        public Dictionary<int, ProjectileFrameData> SetTimelineInformation { set
            {
                keyframes = new();
                frameData = new();
                Dictionary<int, ProjectileFrameData> toBeSorted = value;
                //ideally, the biggest integer key will be the last value in the dict
                SortedDictionary<int, ProjectileFrameData> sorted = new SortedDictionary<int, ProjectileFrameData>(toBeSorted);
                foreach (KeyValuePair<int, ProjectileFrameData> data in sorted)
                {
                    keyframes.Add(data.Key);
                    frameData.Add(data.Value);
                }
            }
        }

        public SortedDictionary<int, ProjectileFrameData> GetTimelineInformation
        {
            get
            {
                Dictionary<int, ProjectileFrameData> toBeSorted = new();
                for(int i = 0; i < keyframes.Count; i++)
                {
                    toBeSorted.Add(keyframes[i], frameData[i]);
                }              
                return new SortedDictionary<int, ProjectileFrameData>(toBeSorted);
            }
        }

        public List<int> Keyframes => keyframes;
        public List<ProjectileFrameData> FrameData => frameData;

    }
    [Serializable]
    public struct ProjectileFrameData
    {
        public ProjectileSnub[] ProjectilesSpawnedAtFrame;
    }
    [Serializable]
    public struct ProjectileSnub
    {
        public Vector3 SpawnPosition, InitialDirection;
        public int ProjectileIndex;
        public override string ToString()
        {
            return $"Spawn Position: {SpawnPosition}, InitialDirection: {InitialDirection}, ProjectileIndex: {ProjectileIndex}";
        }
    }
}
