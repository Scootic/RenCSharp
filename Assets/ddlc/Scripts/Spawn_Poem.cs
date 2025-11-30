using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Spawn_Poem : Screen_Event
    {
        [SerializeField] private Poem poemToSpawn;

        public override void DoShit()
        {
            
        }

        public override string ToString()
        {
            return "Spawn Poem";
        }
    }
}
