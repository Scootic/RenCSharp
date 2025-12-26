using System.Collections.Generic;
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

        public ActorToken(Vector3 pos, string asset, int[] vId)
        {
            XPos = pos.x;
            YPos = pos.y;
            ZPos = pos.z;
            ActorAsset = asset;
            VisualIndexes = vId;
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
