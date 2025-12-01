using System;
using UnityEngine;

namespace RenCSharp
{
    [Serializable]
    public struct ActorToken 
    {
        public float XPos, YPos, ZPos;

        public ActorToken(Vector3 pos)
        {
            XPos = pos.x;
            YPos = pos.y;
            ZPos = pos.z;
        }
    }
}
