using UnityEngine;

namespace RenCSharp.Combat
{
    public class Disarm : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            Debug.LogError("Disarm shout not yet implemented!");
        }
    }
}
