using UnityEngine;
using RenCSharp.Actors;
using System.Collections;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// A simple action that moves an actor toward a position based on the local transform position, over the duration of an IEnumerator using
    /// an animation curve. The animation curve can end at where it starts, meaning simple hop animations are possible.
    /// </summary>
    public class Simple_Actor_Motion : Screen_Event
    {
        [SerializeField] private Vector3 localMotionOffset;
        [SerializeField, Min(0.01f)] private float motionDuration = 1;
        [SerializeField] private AnimationCurve motionPathX, motionPathY, motionPathZ;
        [SerializeField] private Actor target;
        [SerializeField] private bool loopOnScreen = false;
        private float t, eval, dir = 1;
        private GameObject actorObj;
        private Vector3 ogPos, desPos;
        private Coroutine motion;
        public override void DoShit()
        {
            Script_Manager.ProgressScreenEvent += ResetToOG;
            actorObj = GameObject.Find(target.ActorName);
            ogPos = actorObj.transform.position;
            desPos = ogPos + localMotionOffset;
            t = 0;
            motion = Script_Manager.SM.StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            if (!loopOnScreen)
            {
                while (t <= motionDuration) 
                {
                    t += Time.deltaTime;
                    eval = t / motionDuration;
                    SetPosition(eval);
                    yield return null;
                }
            }
            else
            {
                while (true)
                {
                    t += Time.deltaTime * dir;
                    eval = t / motionDuration;
                    if (t > motionDuration) dir = -1;
                    else if (t < 0) dir = 1;
                    SetPosition(eval);
                    yield return null;
                }
            }
        }

        void SetPosition(float eval)
        {
            float x = Mathf.Lerp(ogPos.x, desPos.x, motionPathX.Evaluate(eval));
            float y = Mathf.Lerp(ogPos.y, desPos.y, motionPathY.Evaluate(eval));
            float z = Mathf.Lerp(ogPos.z, desPos.z, motionPathZ.Evaluate(eval));
            actorObj.transform.position = new Vector3(x, y, z);
        }

        private void ResetToOG()
        {
            Script_Manager.SM.StopCoroutine(motion);
            actorObj.transform.position = loopOnScreen ? ogPos : desPos;
        }

        public override string ToString()
        {
            return "Simple Actor Motion";
        }
    }
}
