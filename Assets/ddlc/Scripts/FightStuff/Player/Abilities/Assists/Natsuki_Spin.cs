using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Natsuki_Spin : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.LogError("NATSU SPIN NOT YET IMPLEMENTED");
        }
    }
}
