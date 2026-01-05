using UnityEngine;

namespace RenCSharp.Combat
{
    public class Storm_Call : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.LogError("Storm call not yet implemented");
        }
    }
}
