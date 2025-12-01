using UnityEngine;
using RenCSharp.Actors;
using System;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Take an actor and swap their sprites to something else, based on string references.
    /// </summary>
    [Serializable]
    public class Actor_Expression : Screen_Event
    {
        [SerializeField] private Actor actorToAlter;
        [SerializeField,Tooltip("Reference the strings you put in the actor SO. Empty to skip layer." +
            "Type in the light gray boxes for auto-fill.")] private string[] visualSpriteIndexes = new string[1];
        public override void DoShit()
        {
            if (Object_Factory.TryGetObject(actorToAlter.ActorName, out GameObject spawnt))
            {
                UI_Element uie = spawnt.GetComponent<UI_Element>();
                for (int i = 0; i < visualSpriteIndexes.Length; i++) //loop through all sprites and assign thoroughly, only assign visuals to how many we have
                {
                    if (visualSpriteIndexes[i] == string.Empty) continue; //if you want no change, leave a layer empty
                    uie.Images[i].sprite = actorToAlter.Visuals[i].ReturnSprite(visualSpriteIndexes[i]);
                }
            }
            else Debug.LogWarning("couldn't find actor: " + actorToAlter.ActorName);
        }

        public override string ToString()
        {
            return "Change Actor Expression";
        }
    }
}
