using System.Collections.Generic;
using System;
using UnityEngine;
using RenCSharp.Actors;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Wrapper that only exists for UITK chicanery.
    /// </summary>
    [Serializable]
    public struct VisualIndexes
    {
        public readonly List<List<string>> GetAutoTexts 
        {
            get
            {
                if (ActorToSet == null) return null;

                List<List<string>> toReturn = new();

                foreach (SpriteArray layerindexes in ActorToSet.Visuals)
                {
                    toReturn.Add(layerindexes.visualIDs);
                }

                return toReturn;
            }
        }

        public Actor SetActor
        {
            set
            {
                ActorToSet = value;
                indexes = new List<string>(ActorToSet.Visuals.Length);
            }
        }

        public readonly Sprite SpriteAtIndex(int index, string s)
        {
            return ActorToSet.Visuals[index].ReturnSprite(s);
        }

        public readonly int Length => indexes.Count;
        public List<string> indexes;
        [SerializeField] public Actor ActorToSet;
    }
}
