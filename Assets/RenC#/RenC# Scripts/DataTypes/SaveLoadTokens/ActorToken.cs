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

        public override string ToString()
        {
            string shit = "";

            shit += $"Position: ({XPos}, {YPos}, {ZPos})";
            shit += $"\nActor Asset: {ActorAsset}";
            shit += $"\nVisual Indexes: ";

            foreach(int i in VisualIndexes)
            {
                shit += $"{i}, ";
            }

            return shit;
        }
    }
}
