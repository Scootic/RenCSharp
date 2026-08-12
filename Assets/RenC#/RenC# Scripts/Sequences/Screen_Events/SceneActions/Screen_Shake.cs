using System.Collections;
using UnityEngine;
using UITK_SimpleTimeline;
namespace RenCSharp.Sequences
{
    public class Screen_Shake : Screen_Event
    {
        [Header("Shake Settings")]
        [SerializeField, Tooltip("Decides which axis the background will shake along.")] private ScreenShakeType screenShakeType = ScreenShakeType.Horizontal;
        [SerializeField, Tooltip("Please use only normalized values.")] private AnimationCurve xAxis = Animation_Helper.Jostle;
        [SerializeField, Tooltip("Please use only normalized values.")] private AnimationCurve yAxis = Animation_Helper.Jostle;
        [SerializeField, Tooltip("How far the background will go to the right when the animation curve value is 1.")] private float xOffset = 40f;
        [SerializeField, Tooltip("How far the background will go up when the animation curve value is 1.")] private float yOffset = -40f;
        [SerializeField] private float duration = 0.3f;
        [Header("Scale Image while Shaking")]
        [SerializeField] private bool scaleUp = false;
        [SerializeField, Tooltip("If true, multiplies the animation curve's axis offset value by the corresponding float inside the maxSize vector.")] private bool multiplyOffsetByMaxSize = false;
        [SerializeField] private Vector3 maxSize = new Vector3(1.1f,1.1f,1.1f);
        [SerializeField, Tooltip("Please use only normalized values.")] private AnimationCurve scaleCurve = Animation_Helper.EarlyPeakToZero;

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
                Script_Manager.ProgressScreenEvent += PanicStop;
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

                position = new Vector3(x ? xAxis.Evaluate(perc) * xOffset : 0, y ? yAxis.Evaluate(perc) * yOffset : 0, 0);
                if (multiplyOffsetByMaxSize) position.Set(position.x * maxSize.x, position.y * maxSize.y, position.z * maxSize.z);
                bgTrans.localPosition = position;

                if (scaleUp) bgTrans.localScale = Vector3.Lerp(prevScale, maxSize, scaleCurve.Evaluate(perc));

                yield return null;
            }
        }

        private void PanicStop()
        {
            if(shaker != null) Script_Manager.SM.StopCoroutine(shaker);
            if (bgTrans != null)
            {
                bgTrans.localPosition = Vector3.zero;
                bgTrans.localScale = prevScale;
            }
            Script_Manager.ProgressScreenEvent -= PanicStop;
        }

        public override string ToString()
        {
            return "Scene/Screen Shake";
        }
    }
}
