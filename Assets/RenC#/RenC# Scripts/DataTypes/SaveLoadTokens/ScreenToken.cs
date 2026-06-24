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
        public string MusicAssetKey;
        public string[] BackgroundAssetKeys, OverlayAssetKeys, BackgroundSubobjectKeys, OverlaySubobjectKeys;
        public float BackgroundSPF, OverlaySPF;
        public float[] BackgroundHSC, OverlayHSC, BackgroundColor, OverlayColor;
    }
}
