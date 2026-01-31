using RenCSharp.Actors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RenCSharp.Sequences
{
    public class Group_Simple_Actor_Motion : Screen_Event
    {
        [SerializeField] private Vector3 motionOffset;
        [SerializeField] private float motionDuration;
        [SerializeField] private AnimationCurve xCurve, yCurve, zCurve;
        [SerializeField] private Actor[] targets;
        [SerializeField] private bool loopOnScreen;

        private float t, eval, dir = 1;
        private List<GameObject> actorObjs;
        private Vector3[] ogPos, desPos;
        private Coroutine motion;
        private bool valid;

        public override void DoShit()
        {
            actorObjs = new();
            foreach(Actor a in targets)
            {
                if(Object_Factory.TryGetObject(a.ActorName, out GameObject t))
                {
                    actorObjs.Add(t);
                }
                else
                {
                    Debug.LogWarning("couldn't find actor object for: " + a.ActorName);
                }
            }

            ogPos = new Vector3[actorObjs.Count];
            desPos = new Vector3[actorObjs.Count];

            for(int i = 0; i < actorObjs.Count; i++)
            {
                ogPos[i] = actorObjs[i].transform.position;
                desPos[i] = ogPos[i] + motionOffset;
            }

            t = 0;
            valid = true;
            motion = Script_Manager.SM.StartCoroutine(Animate());
            Script_Manager.ProgressScreenEvent += ResetToOG;
        }

        private IEnumerator Animate()
        {
            if (!loopOnScreen)
            {
                while(t <= motionDuration && valid)
                {
                    t += Time.deltaTime;
                    eval = t / motionDuration;

                    for (int i = 0; i < actorObjs.Count; i++)
                    {
                        actorObjs[i].transform.position = CurveLerp(ogPos[i], desPos[i], eval);
                    }

                    yield return null;
                }

                for (int i = 0; i < actorObjs.Count; i++)
                {
                    actorObjs[i].transform.position = CurveLerp(ogPos[i], desPos[i], 1);
                }
            }
            else
            {
                while (valid)
                {
                    t += Time.deltaTime * dir;
                    eval = t / motionDuration;

                    for (int i = 0; i < actorObjs.Count; i++)
                    {
                        actorObjs[i].transform.position = CurveLerp(ogPos[i], desPos[i], eval);
                    }

                    if (t >= motionDuration) dir = -1;
                    else if (t <= 0) dir = 1;

                    yield return null;
                }

                for (int i = 0; i < actorObjs.Count; i++)
                {
                    actorObjs[i].transform.position = CurveLerp(ogPos[i], desPos[i], 0);
                }
            }
        }

        private Vector3 CurveLerp(Vector3 a, Vector3 b, float eval)
        {
            float x = Mathf.LerpUnclamped(a.x, b.x, xCurve.Evaluate(eval));
            float y = Mathf.LerpUnclamped(a.y, b.y, yCurve.Evaluate(eval));
            float z = Mathf.LerpUnclamped(a.z, b.z, zCurve.Evaluate(eval));
            Vector3 pos = new Vector3(x, y, z);
            return pos;
        }


        private void ResetToOG()
        {
            valid = false;
            for(int i = 0; i < actorObjs.Count; i++)
            {
                float eval = loopOnScreen ? 0 : 1;
                if (actorObjs[i] != null) actorObjs[i].transform.position = CurveLerp(ogPos[i], desPos[i],eval);
            }
        }

        public override string ToString()
        {
            return "Actor/Group Simple Actor Motion";
        }
    }
}
