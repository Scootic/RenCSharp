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
