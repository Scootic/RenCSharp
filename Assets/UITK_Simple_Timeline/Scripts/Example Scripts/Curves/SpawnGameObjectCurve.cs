using System;
using UnityEngine;

namespace UITK_SimpleTimeline
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
        public override string DeleteCurveName() => "Spawn GameObject Curve";
        public override string SpawnKeyframeName() => "Spawn Token Keyframe";
        public override string ToAffectName() => "Prefab to Spawn";

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            TimelineKeyframe<GOSpawnToken>[] keyframes = ClosestTwoKeyframes(time);

            if (!Mathf.Approximately(keyframes[0].Time, time)) return; //make sure the time is comparable!

            GameObject t = GameObject.Instantiate(ToAffect, keyframes[0].Value.SpawnPos, 
                keyframes[0].Value.SpawnRot, keyframes[0].Value.SetToBeChildOfRoot ? root.transform : null);
            t.transform.localScale = keyframes[0].Value.SpawnScale;
            t.name = keyframes[0].Value.name;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Spawn GO Curve is not yet valid!";
            TimelineKeyframe<GOSpawnToken> goose = ClosestTwoKeyframes(time)[0];
            if (!Mathf.Approximately(goose.Time, time))
            {
                return $"Not Spawning GameObject: {ToAffect.name}";
            }
            else
            {
                return $"Spawning GameObject: {ToAffect.name} at, \nPos:{goose.Value.SpawnPos}" +
                    $"\nRot:{goose.Value.SpawnRot}\nScale:{goose.Value.SpawnScale}";
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
    public struct GOSpawnToken
    {
        public Vector3 SpawnPos, SpawnScale;
        public Quaternion SpawnRot;
        public bool SetToBeChildOfRoot;
        public string name;

        public static GOSpawnToken Default
        {
            get
            {
                GOSpawnToken t = new();
                return t;
            }
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
