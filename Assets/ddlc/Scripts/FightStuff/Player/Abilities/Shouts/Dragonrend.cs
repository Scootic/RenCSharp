using UnityEngine;

namespace RenCSharp.Combat
{
    public class Dragonrend : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.LogError("Dragonrend not yet implemented");
        }
    }
}
