
using UnityEngine;

namespace RenCSharp.Sequences
{
    /// <summary>
    /// only use if a poem is already on screen, please.
    /// </summary>
    public class Remove_Poem : Screen_Event
    {
        private bool gone;
        public override void DoShit()
        {
            gone = false;
            RemoveGuy();
            Script_Manager.ProgressScreenEvent += RemoveGuy;
        }

        private void RemoveGuy()
        {
            if (gone) { Debug.Log("I'm already gone"); return; }
            Object_Factory.RemoveObject("Poem");
            gone = true;
        }

        public override string ToString()
        {
            return "Remove Poem";
        }
    }
}
