using RenCSharp.EXPERIMENTAL;
using System.Collections;
using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Player_Action_Defend : Player_Action
    {
        [Header("Defensder")]
        [SerializeField, Range(0, 1)] private float newResistance = 0.1f;
        public override IEnumerator ActionResult()
        {
            Textbox_String.JumpToEndOfTextbox = true;
            if(Object_Factory.TryGetObject("PlayerObject", out GameObject go))
            {
                Event_Bus.TryFireDoubleObjEvent("SetPlayerResistance", (object)false, (object)newResistance);
            }
            yield return null;
        }
    }
}
