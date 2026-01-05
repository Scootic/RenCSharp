using UnityEngine;

namespace RenCSharp.Combat
{
    public class Slow_Time : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.LogError("Slow Time not yet implemented");
        }
    }
}
