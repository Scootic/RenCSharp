using System;
using UnityEngine;
using static UnityEngine.ParticleSystem;
namespace RenCSharp
{
    [Serializable]
    public struct ParticleToken
    {
        public float XPos, YPos, ZPos;
        public string UIParticleGUID, TransformOwner, ParticleName;
        public string[] ParticleSystemGUIDs;
        /// <summary>
        /// constructor! Uses an array of subparticles for a bunch of child effects on one obj.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="uiParticleGUID"></param>
        /// <param name="particleGUIDs"></param>
        /// <param name="particleName"></param>
        /// <param name="transformOwner"></param>
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
        /// <summary>
        /// constructor! only has a single string for a subparticle, only one child effect on the ui particle obj.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="uiParticleGUID"></param>
        /// <param name="subParticleGUID"></param>
        /// <param name="particleName"></param>
        /// <param name="transformOwner"></param>
        public ParticleToken(Vector3 position, string uiParticleGUID, string subParticleGUID, string particleName, string transformOwner = "Overlay")
        {
            XPos = position.x;
            YPos = position.y;
            ZPos = position.z;
            UIParticleGUID = uiParticleGUID;
            string[] t = { subParticleGUID };
            ParticleSystemGUIDs = t;
            ParticleName = particleName;
            TransformOwner = transformOwner;
        }
    }
}
