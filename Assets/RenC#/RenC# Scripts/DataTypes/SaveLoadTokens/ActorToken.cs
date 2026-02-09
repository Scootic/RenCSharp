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
            string stuff = "";

            stuff += $"Position: ({XPos}, {YPos}, {ZPos})";
            stuff += $"\nActor Asset: {ActorAsset}";
            stuff += $"\nVisual Indexes: ";

            foreach(int i in VisualIndexes)
            {
                stuff += $"{i}, ";
            }

            return stuff;
        }
    }
}
