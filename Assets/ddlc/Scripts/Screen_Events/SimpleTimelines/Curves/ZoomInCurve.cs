using UITK_SimpleTimeline;
using UnityEngine;
namespace RenCSharp.Sequences
{
    public class ZoomInCurve : TypedTimelineCurve<ZoomInstruction>
    {
        public override string SpawnKeyframeName() => "Zoom Instruction Keyframe";
        public override string ShorthandCurveName() => "Zoom Curve";
        public override string ToAffectName() => "Nuh uh?";

        private Transform actorParent, bgObj, overlayObj;

        public override void OnPlay()
        {
            //get the actor parent and overlay and background
            if (!Object_Factory.TryGetComponent("Background", out bgObj)) Debug.LogError("ZoomInCurve can't find Background?");
            if (!Object_Factory.TryGetComponent("Actor Holder", out actorParent)) Debug.LogError("ZoomInCurve can't find Actor Holder?");
            if (!Object_Factory.TryGetComponent("Overlay", out overlayObj)) Debug.LogError("ZoomInCurve can't find Overlay?");
        }

        public override ZoomInstruction EvaluateValue(float time)
        {
            TimelineKeyframe<ZoomInstruction>[] keyframes = ClosestTwoKeyframes(time);
            if (!ValidCurve) return new float[4];
            float percTime = TimeToKeyframePercent(time, keyframes[0].Time, keyframes[1].Time);
            float[] tangents = CurveMath.GetTangents(keyframes);
            return CurveMath.CubicHermiteSpline(keyframes[0].Value, keyframes[1].Value,
                percTime, tangents[0], tangents[1], keyframes[0].TangentMode);
        }

        public override void Evaluate(float time)
        {
            ZoomInstruction eval = EvaluateValue(time);
            Vector3 zoomScale = eval.ZoomScale();
            Vector3 actorPos = eval.ScaledActorHolderPosition();
            Vector3 unscaledPos = eval.UnscaledPosition();

            if(!actorPos.HasNaN())actorParent.localPosition = actorPos;
            if (!unscaledPos.HasNaN()) { bgObj.localPosition = unscaledPos; overlayObj.localPosition = unscaledPos; }
            if (!zoomScale.HasNaN()) 
            {
                actorParent.localScale = zoomScale;
                bgObj.localScale = zoomScale;
                overlayObj.localScale = zoomScale; 
            }
        }

        public override string EvaluateMessage(float time)
        {
            return $"Zooming at {time}: {EvaluateValue(time)}";
        }

        public override string ToString()
        {
            return "Camera Zoom Curve";
        }
    }
}
