using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Slow_Time : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.LogError("Slow Time not yet implemented");
        }
    }
}
