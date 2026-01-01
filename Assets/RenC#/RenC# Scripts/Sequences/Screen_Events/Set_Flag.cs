
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Set_Flag : Screen_Event
    {
        [SerializeField] private string flagID = "New Flag";
        [SerializeField] private int flagValue = 0;
        [SerializeField, Tooltip("Decides if you override a flag value, or add to it.")] private bool increment = false;
        [SerializeField,Tooltip("Decides if you are assigning a flag that is per save file, or persistent across all save data.")] private bool persistent = false;

        public override void DoShit()
        {
            if(!increment) Flag_Manager.SetFlag(flagID, flagValue, persistent);
            else Flag_Manager.IncrementFlag(flagID, flagValue, persistent);
        }

        public override string ToString()
        {
            return "Set Flag";
        }
    }
}
