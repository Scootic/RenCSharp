using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Mark_For_Death : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.LogError("Marked for death not yet implemented");
        }
    }
}
