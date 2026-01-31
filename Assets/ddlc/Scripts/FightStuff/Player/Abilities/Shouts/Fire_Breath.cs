using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Fire_Breath : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.LogError("Fire breath not yet implemented");
        }
    }
}
