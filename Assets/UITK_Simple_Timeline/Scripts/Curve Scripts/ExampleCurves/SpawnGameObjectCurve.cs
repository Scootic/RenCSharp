using System;
using UnityEngine;

namespace UITK_SimpleTimeline.Examples
{
    /// <summary>
    /// Example curve that instantiates a Prefab at a position, rotation, with a certain scale, and a specific name.
    /// Notably, doesn't include any sort of function for blending between values; a gameobject is either spawned, or not.<br/><br/>
    /// If you, for some ungodly reason, want to have SimpleTimelines that can spawn GameObjects, you should probably make a version
    /// of this class that is more integrated with your Object Factory or Object Pooling systems. 
    /// (And if you don't have those, you probably should. ESPECIALLY if you're spawning/referencing objects en masse. You don't want
    /// to rely on GameObject.Find().)
    /// This only exists as an example.
    /// </summary>
    public class SpawnGameObjectCurve : TypedTimelineCurve<GOSpawnToken,GameObject>, ILerpable
    {
        protected override int ExcludedKeyframeTangentModes() => 15;
        protected override TimelineKeyframeTangentMode DefaultTangentMode() => TimelineKeyframeTangentMode.Cliff;
      
        public override string ShorthandCurveName() => "Spawn GameObject Curve";
        public override string SpawnKeyframeName() => "Spawn Token Keyframe";
        public override string ToAffectName() => "Prefab to Spawn";

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            GOSpawnToken toSpawn = EvaluateValue(time);
            if (!toSpawn.ActuallySpawn) return;

            GameObject t;
            if (toSpawn.SetToBeChildOfRoot)
            {
                t = GameObject.Instantiate(ToAffect, Vector3.zero, Quaternion.identity, root.transform);
                t.transform.SetLocalPositionAndRotation(toSpawn.SpawnPos, toSpawn.SpawnRot);
            }
            else
            {
                t = GameObject.Instantiate(ToAffect, toSpawn.SpawnPos, toSpawn.SpawnRot);
            }

            t.transform.localScale = toSpawn.SpawnScale;
            t.name = toSpawn.name;
        }

        public override GOSpawnToken EvaluateValue(float time)
        {
            TimelineKeyframe<GOSpawnToken> keyframe = AtTime(time);
            return keyframe.Value;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Spawn GO Curve is not yet valid!";
            try
            {
                    GOSpawnToken toEval = EvaluateValue(time);
                    return $"Spawned GameObject: {ToAffect.name} at, \n\tPos:{toEval.SpawnPos}" +
                        $"\n\tRot:{toEval.SpawnRot}\n\tScale:{toEval.SpawnScale}";
                }
                catch
                {
                    return $"Not Spawning GameObject: {ToAffect.name}";
                }

        }
        
        public override string ToString()
        {
            return "Spawn GameObject Curve";
        }
    }
    /// <summary>
    /// A struct holding all the information a GameObject should need to be instantiated.
    /// </summary>
    [Serializable]
    public struct GOSpawnToken : IDefaultableNotNull<GOSpawnToken>
    {
        [Tooltip("Pretty please set me to true...")]public bool ActuallySpawn;
        public Vector3 SpawnPos;
        public Vector3 SpawnScale;
        public Quaternion SpawnRot;
        [Tooltip("Decides if the spawned prefab should be made a child of the root object of the timeline.")]public bool SetToBeChildOfRoot;
        [Tooltip("The name the spawned GO will have when first spawned. Will likely be appended to prevent duplicates.")]public string name;

        public readonly GOSpawnToken Default()
        {
            return new()
            {
                ActuallySpawn = false,
                SpawnPos = Vector3.zero,
                SpawnRot = Quaternion.identity,
                SpawnScale = Vector3.one,
                SetToBeChildOfRoot = false,
                name = "GameObject"
            };
        }
        

        public readonly override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public readonly override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
