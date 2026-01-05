using UnityEngine;

namespace RenCSharp.Combat
{
    public class Mark_For_Death : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            Debug.LogError("Marked for death not yet implemented");
        }
    }
}
