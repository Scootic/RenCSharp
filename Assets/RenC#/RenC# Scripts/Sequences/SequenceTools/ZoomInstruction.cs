using RenCSharp.Actors;
using System;
using UITK_SimpleTimeline;
using UnityEngine;

namespace RenCSharp.Sequences
{
    [Serializable]
    public struct ZoomInstruction : IDefaultableNotNull<ZoomInstruction>
    {
        [Tooltip("Overrides xPos, yPos, and zPos if present. Set to null if you want to manually set positions. (For example, 0,0,0 for un-zoomed origin.)")] public Actor ActorToFocus;
        public float xPos, yPos, zPos;
        public float zoomScale;

        public readonly ZoomInstruction Default()
        {
            return new()
            {
                ActorToFocus = null,
                xPos = 0,
                yPos = 0,
                zPos = 0,
                zoomScale = 1
            };
        }

        public readonly override string ToString()
        {
            return $"Actor: {(ActorToFocus ? ActorToFocus.name : "No Actor")}, Position: ({xPos},{yPos},{zPos}), Zoom Scale: {zoomScale}";
        }

        public ZoomInstruction(Actor act, float[] floara)
        {
            ActorToFocus = act;
            xPos = floara[0];
            yPos = floara[1];
            zPos = floara[2];
            zoomScale = floara[3];
        }

        public Vector3 UnscaledPosition()
        {
            if (ActorToFocus != null && Object_Factory.TryGetComponent(ActorToFocus.name, out Transform t))
            {
                Vector3 negLPos = -t.localPosition;
                xPos = negLPos.x;
                yPos = negLPos.y;
                zPos = negLPos.z;
            }
            return new() { x = xPos, y = yPos, z = zPos };
        }

        public Vector3 ScaledActorHolderPosition()
        {
            if (ActorToFocus != null && Object_Factory.TryGetComponent(ActorToFocus.name, out Transform t))
            {
                Vector3 negLPos = -t.localPosition;
                xPos = negLPos.x;
                yPos = negLPos.y;
                zPos = negLPos.z;
            }
            return new() { x = xPos * zoomScale, y = yPos * zoomScale, z = zPos * zoomScale };
        }

        public readonly Vector3 ZoomScale()
        {
            return new() { x = zoomScale, y = zoomScale, z = zoomScale };
        }

        public static implicit operator float[](ZoomInstruction zi)
        {
            if (zi.ActorToFocus != null && Object_Factory.TryGetComponent(zi.ActorToFocus.name, out Transform t))
            {
                Vector3 negLPos = -t.localPosition;
                zi.xPos = negLPos.x;
                zi.yPos = negLPos.y;
                zi.zPos = negLPos.z;
            }
            return new float[] { zi.xPos, zi.yPos, zi.zPos, zi.zoomScale };
        }

        public static implicit operator ZoomInstruction(float[] f)
        {
            if (f.Length != 4)
            {
                Debug.LogWarning("Attempting to implicitly convert a float array to ZoomInstruction that doesn't have a length" +
                    " of 4. Returning an empty ZoomInstruction.");
                return new();
            }
            return new() { xPos = f[0], yPos = f[1], zPos = f[2], zoomScale = f[3] };
        }
    }
}
