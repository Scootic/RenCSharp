using UnityEngine;

namespace RenCSharp.Combat
{
    public class Fire_Breath : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.LogError("Fire breath not yet implemented");
        }
    }
}
