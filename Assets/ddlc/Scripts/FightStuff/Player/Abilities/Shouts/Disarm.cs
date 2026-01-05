using UnityEngine;

namespace RenCSharp.Combat
{
    public class Disarm : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.LogError("Disarm shout not yet implemented!");
        }
    }
}
