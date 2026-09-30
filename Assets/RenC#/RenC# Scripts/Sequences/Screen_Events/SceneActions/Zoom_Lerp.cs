using System.Collections;
using UnityEngine;
namespace RenCSharp.Sequences
{
    public class Zoom_Lerp : Screen_Event
    {
        [SerializeField] private ZoomInstruction toZoomOn;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float zoomDuration = 1;

        private Coroutine routine;
        private Transform actors, bg, ov;
        public override void DoEvent()
        {
            Sequence_Manager.ProgressScreenEvent += PanicStop;
            routine = Sequence_Manager.SM.StartCoroutine(Zoom());
        }

        private IEnumerator Zoom()
        {
            float perc;
            float t = 0;

            if(!Object_Factory.TryGetComponent("Actor Holder", out actors)) yield break;
            if(!Object_Factory.TryGetComponent("Background", out bg)) yield break;
            if(!Object_Factory.TryGetComponent("Overlay", out ov)) yield break;

            Vector3 ogActPos = actors.localPosition;
            Vector3 ogBGPos = bg.localPosition;
            Vector3 ogOVPos = ov.localPosition;
            //we don't need individual scales, since they will be shared.
            Vector3 ogScale = actors.localScale;
            Vector3 scaleLerp;

            while(t < zoomDuration)
            {
                t += Time.deltaTime;
                perc = curve.Evaluate(t / zoomDuration);

                actors.localPosition = Vector3.Lerp(ogActPos, toZoomOn.ScaledActorHolderPosition(), perc);
                bg.localPosition = Vector3.Lerp(ogBGPos, toZoomOn.UnscaledPosition(), perc);
                ov.localPosition = Vector3.Lerp(ogOVPos, toZoomOn.UnscaledPosition(), perc);

                scaleLerp = Vector3.Lerp(ogScale, toZoomOn.ZoomScale(), perc);

                actors.localScale = scaleLerp;
                bg.localScale = scaleLerp;
                ov.localScale = scaleLerp;

                yield return null;
            }

            actors.localPosition = toZoomOn.ScaledActorHolderPosition();
            bg.localPosition = toZoomOn.UnscaledPosition();
            ov.localPosition = toZoomOn.UnscaledPosition();

            actors.localScale = toZoomOn.ZoomScale();
            bg.localScale = toZoomOn.ZoomScale();
            ov.localScale = toZoomOn.ZoomScale();
        }

        private void PanicStop()
        {
            Sequence_Manager.ProgressScreenEvent -= PanicStop;
            Sequence_Manager.SM.StopCoroutine(routine);

            actors.localPosition = toZoomOn.ScaledActorHolderPosition();
            bg.localPosition = toZoomOn.UnscaledPosition();
            ov.localPosition = toZoomOn.UnscaledPosition();

            actors.localScale = toZoomOn.ZoomScale();
            bg.localScale = toZoomOn.ZoomScale();
            ov.localScale = toZoomOn.ZoomScale();
        }

        public override string ToString()
        {
            return "Scene/Focus Actor or Position";
        }
    }
}
