using System.Collections.Generic;
using System;

namespace RenCSharp
{
    [Serializable]
    public struct ScreenToken 
    {
        public List<ActorToken> ActiveActors;
        public string MusicAssetKey;
        public string[] BackgroundAssetKeys, OverlayAssetKeys, BackgroundSubobjectKeys, OverlaySubobjectKeys;
        public float BackgroundSPF, OverlaySPF;
    }
}
