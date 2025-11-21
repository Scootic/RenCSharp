using UnityEngine;
using RenCSharp.Actors;
using UnityEngine.UI;
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
        [SerializeField,Tooltip("Reference the strings you put in the actor SO. Empty to skip layer.")] private string[] visualSpriteIndexes = new string[1];
        public override void DoShit()
        {
            GameObject spawnt = GameObject.Find(actorToAlter.ActorName);
            if (spawnt != null)
            {
                Image image = spawnt.transform.GetChild(0).GetComponent<Image>();
                for (int i = 0; i < visualSpriteIndexes.Length; i++) //loop through all sprites and assign thoroughly, only assign visuals to how many we have
                {
                    if (visualSpriteIndexes[i] == string.Empty) continue; //means you can put -1 as a visalspriteindex if you want no change
                    image.sprite = actorToAlter.Visuals[i].ReturnSprite(visualSpriteIndexes[i]);
                    if (i < visualSpriteIndexes.Length - 1) image = image.transform.GetChild(0).GetComponent<Image>(); //grab child for next step
                }
            }
            else
            {
                Debug.LogWarning("couldn't find actor: " + actorToAlter.ActorName);
            }
        }

        public override string ToString()
        {
            return "Change Actor Expression";
        }
    }
}
