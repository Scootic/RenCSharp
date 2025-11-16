using UnityEngine;

namespace RenCSharp.Sequences
{
    /// <summary>
    /// A simple action that moves an actor toward a position based on the local transform position, over the duration of an IEnumerator using
    /// an animation curve. The animation curve can end at where it starts, meaning simple hop animations are possible.
    /// </summary>
    public class Simple_Actor_Motion : Screen_Event
    {
        [SerializeField] private Vector3 localMotionOffset;
        [SerializeField] private float motionDuration;
        [SerializeField] private AnimationCurve motionPath;
        [SerializeField] private Actor target;
        private float t;
        private GameObject actorObj;
        private Vector3 ogPos, desPos;
        public override void DoShit()
        {
            actorObj = GameObject.Find(target.ActorName);
            ogPos = actorObj.transform.position;
            desPos = ogPos + localMotionOffset;
            t = 0;
        }
    }
}
