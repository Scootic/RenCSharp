using UnityEngine;

namespace RenCSharp.Combat
{
    public class Natsuki_Spin : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.LogError("NATSU SPIN NOT YET IMPLEMENTED");
        }
    }
}
