using UnityEngine;
using RenCSharp.Actors;
using UnityEngine.UI;
using System;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Spawns an actor into the scene using an Actor SO, an index that is passed to the Script_Manager as a place to spawn, and indexes for
    /// the appearance that the actor will take on.
    /// </summary>
    [Serializable]
    public class Spawn_Actor : Screen_Event
    {
        [SerializeField] private Actor actorToSpawn;
        [SerializeField] private Vector3 spawnOffset = Vector3.zero;
        [SerializeField] private string[] visualSpriteIndexes = new string[1];
        public override void DoShit()
        {
            Transform placeWeWant = Script_Manager.SM.ActorHolder;
            GameObject spawnt = GameObject.Instantiate(actorToSpawn.ActorPrefab, placeWeWant);
            spawnt.transform.position += spawnOffset;
            spawnt.name = actorToSpawn.ActorName;
            Image image = spawnt.transform.GetChild(0).GetComponent<Image>(); 
            for (int i = 0; i < visualSpriteIndexes.Length; i++) //loop through all sprites and assign thoroughly, only assign visuals to how many we have
            {
                image.sprite = actorToSpawn.Visuals[i].ReturnSprite(visualSpriteIndexes[i]);
                if(i < visualSpriteIndexes.Length - 1)image = image.transform.GetChild(0).GetComponent<Image>();
            }
        }

        public override string ToString()
        {
            return "Spawn Actor";
        }
    }
}
