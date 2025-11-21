
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Set_Flag : Screen_Event
    {
        [SerializeField] private string flagID = "New Flag";
        [SerializeField] private int flagValue = 0;

        public override void DoShit()
        {
            Script_Manager.SM.SetFlag(flagID, flagValue);
        }

        public override string ToString()
        {
            return "Set Flag";
        }
    }
}
