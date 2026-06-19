using System.Collections;
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Screen_Shake : Screen_Event
    {
        [SerializeField] private ScreenShakeType screenShakeType = ScreenShakeType.BothAxis;
        [SerializeField] private AnimationCurve xAxis, yAxis;
        [SerializeField] private float duration;

        private Coroutine shaker;
        private float t, perc;
        private Transform bgTrans;
        public override void DoEvent()
        {
            if(Object_Factory.TryGetObject("Background", out GameObject go))
            {
                bgTrans = go.transform;
                shaker = Script_Manager.SM.StartCoroutine(ShakeThatScreen());
            }
            else
            {
                Debug.LogWarning("Screen shake couldn't find the background!");
            }
        }

        private IEnumerator ShakeThatScreen()
        {
            t = 0;
            Vector3 position;
            bool x = screenShakeType == ScreenShakeType.Horizontal || screenShakeType == ScreenShakeType.BothAxis;
            bool y = screenShakeType == ScreenShakeType.Vertical || screenShakeType == ScreenShakeType.BothAxis;

            while (t < duration)
            {
                t += Time.deltaTime;
                perc = t / duration;

                position = new Vector3(x ? xAxis.Evaluate(perc) : 0, y ? yAxis.Evaluate(perc) : 0, 0);
                bgTrans.localPosition = position;

                yield return null;
            }
        }

        private void PanicStop()
        {
            Script_Manager.SM.StopCoroutine(shaker);
            bgTrans.localPosition = Vector3.zero;
        }

        public override string ToString()
        {
            return "Screen Shake";
        }
    }
}
