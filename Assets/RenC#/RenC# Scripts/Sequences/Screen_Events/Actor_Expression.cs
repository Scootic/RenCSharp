using UnityEngine;
using RenCSharp.Actors;
using UnityEngine.UI;
using System;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Take an actor and swap their sprites to something else, based on indexes.
    /// </summary>
    [Serializable]
    public class Actor_Expression : Screen_Event
    {
        [SerializeField] private Actor actorToAlter;
        [SerializeField] private int[] visualSpriteIndexes = new int[1];
        public override void DoShit()
        {
            GameObject spawnt = GameObject.Find(actorToAlter.ActorName);
            if (spawnt != null)
            {
                Image image = spawnt.transform.GetChild(0).GetComponent<Image>();
                for (int i = 0; i < visualSpriteIndexes.Length; i++) //loop through all sprites and assign thoroughly, only assign visuals to how many we have
                {
                    image.sprite = actorToAlter.Visuals[i].layer[visualSpriteIndexes[i]];
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
