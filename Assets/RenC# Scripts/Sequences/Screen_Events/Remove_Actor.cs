using UnityEngine;
using RenCSharp.Actors;
using System;
namespace RenCSharp.Sequences
{
    [Serializable]
    public class Remove_Actor: Screen_Event
    {
        [SerializeField] private Actor actorToRemove;

        public override void DoShit()
        {
            GameObject fellaToRemove = GameObject.Find(actorToRemove.ActorName);
            if (fellaToRemove != null)
            {
                GameObject.Destroy(fellaToRemove);
            }
            else
            {
                Debug.LogWarning("Did not find actor: " + actorToRemove.ActorName);
            }
        }

        public override string ToString()
        {
            return "Remove Actor";
        }
    }
}
