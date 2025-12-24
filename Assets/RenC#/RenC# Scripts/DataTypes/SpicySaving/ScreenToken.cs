using System.Collections.Generic;
using System;

namespace RenCSharp
{
    [Serializable]
    public struct ScreenToken 
    {
        public List<ActorToken> ActiveActors;
        public int MusicAssetIndex;
        public int[] BackgroundAssetIndexes, OverlayAssetIndexes;
        public float BackgroundSPF, OverlaySPF;
    }
}
