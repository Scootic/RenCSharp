using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Disarm : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.LogError("Disarm shout not yet implemented!");
        }
    }
}
