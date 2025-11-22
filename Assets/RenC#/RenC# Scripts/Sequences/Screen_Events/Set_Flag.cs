
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Set_Flag : Screen_Event
    {
        [SerializeField] private string flagID = "New Flag";
        [SerializeField] private int flagValue = 0;
        [SerializeField, Tooltip("Decides if you override a flag value, or add to it.")] private bool increment = false;

        public override void DoShit()
        {
            if(!increment) Script_Manager.SM.SetFlag(flagID, flagValue);
            else Script_Manager.SM.IncrementFlag(flagID, flagValue);
        }

        public override string ToString()
        {
            return "Set Flag";
        }
    }
}
