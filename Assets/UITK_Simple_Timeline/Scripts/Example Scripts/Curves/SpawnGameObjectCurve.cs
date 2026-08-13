using System;
using UnityEngine;

namespace UITK_SimpleTimeline
{
    /// <summary>
    /// Example curve that instantiates a Prefab at a position, rotation, with a certain scale, and a specific name.
    /// Notably, doesn't include any sort of function for blending between values; a gameobject is either spawned, or not.<br/><br/>
    /// If you, for some ungodly reason, want to have SimpleTimelines that can spawn GameObjects, you should probably make a version
    /// of this class that is more integrated with your Object Factory or Object Pooling systems. (And if you don't have those, you should.)
    /// This only exists as an example.
    /// </summary>
    public class SpawnGameObjectCurve : TimelineCurve<GOSpawnToken,GameObject>
    {
        public override GOSpawnToken Evaluate(float time)
        {
            TimelineKeyframe<GOSpawnToken>[] keyframes = ClosestTwoKeyframes(time);

            if (!Mathf.Approximately(keyframes[0].Time, time)) return new GOSpawnToken(); //make sure the time is comparable!

            GameObject t = GameObject.Instantiate(ToAffect, keyframes[0].Value.GetPosition(), keyframes[0].Value.GetRotation());
            t.transform.localScale = keyframes[0].Value.GetScale();
            t.name = keyframes[0].Value.name;

            return keyframes[0].Value;
        }
    }

    [Serializable]
    public struct GOSpawnToken
    {
        public float xPos, yPos, zPos;
        public float xRot, yRot, zRot, wRot;
        public float xScale, yScale, zScale;
        public string name;

        public readonly Vector3 GetPosition() { return new Vector3(xPos, yPos, zPos); }

        public readonly Quaternion GetRotation() { return new Quaternion(xRot, yRot, zRot, wRot); }
        public readonly Vector3 GetScale() { return new Vector3(xScale, yScale, zScale); }
    }
}
