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
        public List<Sprite> layer;
        public List<string> visualIDs;

        public Sprite ReturnSprite(string id)
        {
            Sprite fellow = null;
            int i = 0;

            if (visualIDs.Contains(id)) { i = visualIDs.IndexOf(id); fellow = layer[i]; }
            else Debug.LogWarning("No sprite found at index: " + id); 

            return fellow;
        }
    }
}
