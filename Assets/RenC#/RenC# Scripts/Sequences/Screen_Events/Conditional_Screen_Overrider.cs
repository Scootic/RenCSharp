using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Conditional_Screen_Overrider : Screen_Event
    {
        [SerializeField] private ConditionalScreen[] possibleScreens;
        public override void DoShit()
        {
            foreach(ConditionalScreen cs in possibleScreens)
            {
                if (cs.MetAllConditions()) //override with the first valid screen.
                {
                    object t = (object)cs.ResultingScreen;
                    Event_Bus.TryFireSingleObjEvent("OverrideScreen", t);
                    break;
                }
            }
        }

        public override string ToString()
        {
            return "Conditional Screen";
        }
    }
}
