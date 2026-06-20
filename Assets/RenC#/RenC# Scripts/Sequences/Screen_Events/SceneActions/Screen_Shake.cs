using System.Collections;
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Screen_Shake : Screen_Event
    {
        [Header("Shake Settings")]
        [SerializeField, Tooltip("Decides which axis the background will shake along.")] private ScreenShakeType screenShakeType = ScreenShakeType.BothAxis;
        [SerializeField, Tooltip("You don't need to use normalized values, but still 0 - 1 time values.")] private AnimationCurve xAxis;
        [SerializeField, Tooltip("You don't need to use normalized values, but still 0 - 1 time values.")] private AnimationCurve yAxis;
        [SerializeField] private float duration = 0.5f;
        [Header("Scale Image while Shaking")]
        [SerializeField] private bool scaleUp = false;
        [SerializeField] private Vector3 maxSize = Vector3.one;
        [SerializeField, Tooltip("Please use only normalized values.")] private AnimationCurve scaleCurve;

        private Coroutine shaker;
        private float t, perc;
        private Vector3 position, prevScale;
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
            prevScale = bgTrans.transform.localScale;
            bool x = screenShakeType == ScreenShakeType.Horizontal || screenShakeType == ScreenShakeType.BothAxis;
            bool y = screenShakeType == ScreenShakeType.Vertical || screenShakeType == ScreenShakeType.BothAxis;

            while (t < duration)
            {
                t += Time.deltaTime;
                perc = t / duration;

                position = new Vector3(x ? xAxis.Evaluate(perc) : 0, y ? yAxis.Evaluate(perc) : 0, 0);
                bgTrans.localPosition = position;

                if (scaleUp) bgTrans.localScale = Vector3.Lerp(prevScale, maxSize, scaleCurve.Evaluate(perc));

                yield return null;
            }
        }

        private void PanicStop()
        {
            Script_Manager.SM.StopCoroutine(shaker);
            bgTrans.localPosition = Vector3.zero;
            bgTrans.localScale = prevScale;
        }

        public override string ToString()
        {
            return "Screen Shake";
        }
    }
}
