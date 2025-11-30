using UnityEngine;

namespace RenCSharp.Sequences
{
    /// <summary>
    /// only use if a poem is already on screen, please.
    /// </summary>
    public class Remove_Poem : Screen_Event
    {
        public override void DoShit()
        {
            Object_Factory.RemoveObject("Poem");
        }

        public override string ToString()
        {
            return "Remove Poem";
        }
    }
}
