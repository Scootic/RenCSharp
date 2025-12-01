using System.Collections.Generic;
using System;

namespace RenCSharp
{
    [Serializable]
    public struct ScreenToken 
    {
        public List<ActorToken> ActiveActors;
        public string BackgroundAsset, MusicAsset, OverlayAsset;
    }
}
