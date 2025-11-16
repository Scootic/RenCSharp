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
        [SerializeField] private int transformIndex;
        [SerializeField] private int[] visualSpriteIndexes = new int[1];
        public override void DoShit()
        {
            GameObject spawnt = GameObject.Instantiate(actorToSpawn.ActorPrefab, Script_Manager.SM.ActorPositions[transformIndex].position, Quaternion.identity);
            spawnt.name = actorToSpawn.ActorName;
            Image image = spawnt.transform.GetChild(0).GetComponent<Image>(); 
            for (int i = 0; i < visualSpriteIndexes.Length; i++) //loop through all sprites and assign thoroughly, only assign visuals to how many we have
            {
                image.sprite = actorToSpawn.Visuals[i].layer[visualSpriteIndexes[i]];
                image = image.transform.GetChild(0).GetComponent<Image>();
            }
        }

        public override string ToString()
        {
            return "Spawn Actor";
        }
    }
}
