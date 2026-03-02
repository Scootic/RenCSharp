using UnityEngine;
using System;
namespace RenCSharp
{
    [Serializable]
    public struct ParticleToken
    {
        public float XPos, YPos, ZPos;
        public string UIParticleGUID, TransformOwner, ParticleName;
        public string[] ParticleSystemGUIDs;

        public ParticleToken(Vector3 position, string uiParticleGUID, string[] particleGUIDs, string particleName, string transformOwner = "Overlay")
        {
            XPos = position.x;
            YPos = position.y;
            ZPos = position.z;
            UIParticleGUID = uiParticleGUID;
            ParticleSystemGUIDs = particleGUIDs;
            ParticleName = particleName;
            TransformOwner = transformOwner;
        }
    }
}
