using System;
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
            float[] tangents = CurveMath.GetTangents(keyframes);
            return CurveMath.CubicHermiteSpline(keyframes[0].Value, keyframes[1].Value,
                TimeToKeyframePercent(time, keyframes[0].Time, keyframes[1].Time), tangents[0], tangents[1], keyframes[0].TangentMode);
        }

        public override void Evaluate(float time)
        {
            ZoomInstruction eval = EvaluateValue(time);
            actorParent.localPosition = eval.ScaledActorHolderPosition();
            actorParent.localScale = eval.ZoomScale();

            bgObj.localPosition = eval.UnscaledPosition();
            bgObj.localScale = eval.ZoomScale();

            overlayObj.localPosition = eval.UnscaledPosition();
            overlayObj.localScale = eval.ZoomScale();
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
    [Serializable]
    public struct ZoomInstruction : IDefaultableNotNull<ZoomInstruction>
    {
        public float xPos, yPos, zPos;
        public float zoomScale;

        public readonly ZoomInstruction Default()
        {
            return new()
            {
                xPos = 0,
                yPos = 0,
                zPos = 0,
                zoomScale = 1
            };
        }

        public readonly Vector3 UnscaledPosition()
        {
            return new() { x = xPos, y = yPos, z = zPos };
        }

        public readonly Vector3 ScaledActorHolderPosition()
        {
            return new() { x = xPos * zoomScale, y = yPos * zoomScale, z = zPos * zoomScale };
        }

        public readonly Vector3 ZoomScale()
        {
            return new() { x = zoomScale, y = zoomScale, z = zoomScale };
        }

        public static implicit operator float[](ZoomInstruction zi)
        {
            return new float[] { zi.xPos, zi.yPos, zi.zPos, zi.zoomScale};
        }

        public static implicit operator ZoomInstruction(float[] f)
        {
            if (f.Length != 4) { 
                Debug.LogWarning("Attempting to implicitly convert a float array to ZoomInstruction that doesn't have a length" +
                    " of 4. Returning an empty ZoomInstruction."); 
                return new(); }
            return new() { xPos = f[0],yPos = f[1],zPos = f[2], zoomScale = f[3] };
        }
    }
}
