using UnityEngine;

namespace RenCSharp.Combat.Player
{   
    public class Random_BS : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.LogError("RandomBS not yet implemented");
        }
    }
}
