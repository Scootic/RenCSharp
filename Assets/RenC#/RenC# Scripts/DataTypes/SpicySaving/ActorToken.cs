using System;
using UnityEngine;

namespace RenCSharp
{
    [Serializable]
    public struct ActorToken 
    {
        public float XPos, YPos, ZPos;
        public string ActorAsset;
        public int[] VisualIndexes;
        public ActorToken(Vector3 pos, string ActorSO, int[] visualIndexes)
        {
            XPos = pos.x;
            YPos = pos.y;
            ZPos = pos.z;
            ActorAsset = ActorSO;
            VisualIndexes = visualIndexes;
        }
    }
}
