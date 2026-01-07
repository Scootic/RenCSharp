using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Storm_Call : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.LogError("Storm call not yet implemented");
        }
    }
}
