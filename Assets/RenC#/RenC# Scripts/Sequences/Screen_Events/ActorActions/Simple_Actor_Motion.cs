using RenCSharp.Actors;
using System.Collections;
using UnityEngine;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// A simple action that moves an actor toward a position based on the local transform position, over the duration of an IEnumerator using
    /// an animation curve. The animation curve can end at where it starts, meaning simple hop animations are possible. Additionally, the lerping is
    /// unclamped, meaning you can have negative values in your animation curve.
    /// </summary>
    public class Simple_Actor_Motion : Screen_Event
    {
        [SerializeField] protected Vector3 localMotionOffset;
        [SerializeField, Min(0.01f)] protected float motionDuration = 1;
        [SerializeField] protected AnimationCurve motionPathX, motionPathY, motionPathZ;
        [SerializeField] private Actor target;
        [SerializeField] protected bool loopOnScreen = false;
        protected float t, eval, dir = 1;
        private GameObject actorObj;
        private Vector3 ogPos, desPos;
        protected Coroutine motion;
        protected bool valid;
        public override void DoShit()
        {
            if (Object_Factory.TryGetObject(target.ActorName, out actorObj))
            {
                ogPos = actorObj.transform.position;
                desPos = ogPos + localMotionOffset;
                t = 0;
                valid = true;
                motion = Script_Manager.SM.StartCoroutine(Animate());
                Script_Manager.ProgressScreenEvent += ResetToOG;
            }
            else
            {
                Debug.LogWarning("couldn't find actor object: " + target.ActorName);
            }
        }

        protected virtual IEnumerator Animate()
        {
            if (!loopOnScreen) //if no loop, end at the end of the curve
            {
                while (t <= motionDuration && valid) 
                {
                    t += Time.deltaTime;
                    eval = t / motionDuration;
                    actorObj.transform.position = SetPosition(eval);
                    yield return null;
                }
                if (actorObj != null) actorObj.transform.position = SetPosition(1f);
            }
            else //if we DO loop, end at start of curve
            {
                while (valid)
                {
                    t += Time.deltaTime * dir;
                    eval = t / motionDuration;
                    if (t > motionDuration) dir = -1;
                    else if (t < 0) dir = 1;
                    actorObj.transform.position = SetPosition(eval);
                    yield return null;
                }
                if(actorObj != null) actorObj.transform.position = SetPosition(0f);
            }
        }

        protected Vector3 SetPosition(float eval)
        {
            Vector3 pos;
            float x = Mathf.LerpUnclamped(ogPos.x, desPos.x, motionPathX.Evaluate(eval));
            float y = Mathf.LerpUnclamped(ogPos.y, desPos.y, motionPathY.Evaluate(eval));
            float z = Mathf.LerpUnclamped(ogPos.z, desPos.z, motionPathZ.Evaluate(eval));
            pos = new Vector3(x, y, z);
            return pos;
        }

        protected virtual void ResetToOG()
        {
            valid = false;
            if (actorObj == null) return;
            if (loopOnScreen) actorObj.transform.position = SetPosition(0f);
            else actorObj.transform.position = SetPosition(1f);
        }

        public override string ToString()
        {
            return "Actor/Simple Actor Motion";
        }
    }
}
