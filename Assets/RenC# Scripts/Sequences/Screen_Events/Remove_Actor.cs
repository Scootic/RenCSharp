using UnityEngine;
using RenCSharp.Actors;
using System;
namespace RenCSharp.Sequences
{
    [Serializable]
    public class Remove_Actor: Screen_Event
    {
        [SerializeField] private Actor bitchToRemove;

        public override void DoShit()
        {
            GameObject fellaToRemove = GameObject.Find(bitchToRemove.ActorName);
            if (fellaToRemove != null)
            {
                GameObject.Destroy(fellaToRemove);
            }
            else
            {
                Debug.LogWarning("Did not find actor: " + bitchToRemove.ActorName);
            }
        }

        public override string ToString()
        {
            return "Remove Actor";
        }
    }
}
