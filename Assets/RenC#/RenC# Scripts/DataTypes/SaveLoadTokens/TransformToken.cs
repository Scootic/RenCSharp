using System;
using UnityEngine;

namespace RenCSharp
{
    [Serializable]
    public readonly struct TransformToken
    {
        private readonly float LocalXPos, LocalYPos, LocalZPos;
        private readonly float LocalXScale, LocalYScale, LocalZScale;
        private readonly float LocalXRot, LocalYRot, LocalZRot, LocalWRot;

        public readonly Vector3 LocalPos => new(LocalXPos, LocalYPos, LocalZPos);
        public readonly Vector3 LocalScale => new(LocalXScale, LocalYScale, LocalZScale);
        public readonly Quaternion LocalRot => new(LocalXRot, LocalYRot, LocalZRot, LocalWRot);

        public TransformToken(Transform t)
        {
            LocalXPos = t.localPosition.x;
            LocalYPos = t.localPosition.y;
            LocalZPos = t.localPosition.z;

            LocalXScale = t.localScale.x;
            LocalYScale = t.localScale.y;
            LocalZScale = t.localScale.z;

            LocalXRot = t.localRotation.x;
            LocalYRot = t.localRotation.y;
            LocalZRot = t.localRotation.z;
            LocalWRot = t.localRotation.w;
        }
    }
}
