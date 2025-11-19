using System;
using UnityEngine;

namespace RenCSharp
{
    /// <summary>
    /// A wrapper for actors to have a '2D' sprite array in inspector. Thx Obama.
    /// </summary>
    [Serializable]
    public struct SpriteArray 
    {
        public Sprite[] layer;
        public string[] visualIDs;
    }
}
