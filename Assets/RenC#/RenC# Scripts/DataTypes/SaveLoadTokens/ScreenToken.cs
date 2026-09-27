using System.Collections.Generic;
using System;
namespace RenCSharp
{
    [Serializable]
    public struct ScreenToken 
    {
        public List<ActorToken> ActiveActors;
        public List<ParticleToken> ActiveParticles;
        public List<SFXToken> ActiveESFX;
        /// <summary>
        /// 0 for Actor Holder, 1 for Background, 2 for Overlay.
        /// </summary>
        public TransformToken[] MovingTransforms;

        public string MusicAssetKey;
        public string[] BackgroundAssetKeys, OverlayAssetKeys, BackgroundSubobjectKeys, OverlaySubobjectKeys;
        public float BackgroundSPF, OverlaySPF;
        public float[] BackgroundHSC, OverlayHSC, BackgroundColor, OverlayColor;
    }
}
