using System;
using UnityEngine;
using System.Collections.Generic;
namespace RenCSharp
{
    /// <summary>
    /// A wrapper for actors to have a '2D' sprite array in inspector. Thx Obama. Not even an array anymore. This bastard is
    /// an overglorified dictionary.
    /// </summary>
    [Serializable]
    public struct SpriteArray 
    {
        public Sprite[] layer;
        public string[] visualIDs;

        public Sprite ReturnSprite(string id)
        {
            Sprite fellow = null;
            int i = 0;
            return fellow;
        }
    }
}
