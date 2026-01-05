using UnityEngine;

namespace RenCSharp.Combat
{
    public class Random_BS : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.LogError("RandomBS not yet implemented");
        }
    }
}
