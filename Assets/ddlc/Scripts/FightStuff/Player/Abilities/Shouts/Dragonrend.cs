using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Dragonrend : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.LogError("Dragonrend not yet implemented");
        }
    }
}
