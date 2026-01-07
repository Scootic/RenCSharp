using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Unrelenting_Force : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.LogError("Unrelenting Force not yet implemented");
        }
    }
}
