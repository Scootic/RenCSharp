using System;
using UnityEngine;

namespace RenCSharp.Sequences
{
    [Serializable]
    public class Screen_Event
    {
        [SerializeField] private int scumFuck;
        [SerializeField] private Spawn_Actor spa;
        //[SerializeReference] private Screen_Event child;
        public virtual void DoShit() { }
    }
}
