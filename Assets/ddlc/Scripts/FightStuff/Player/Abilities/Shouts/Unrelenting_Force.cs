using UnityEngine;

namespace RenCSharp.Combat
{
    public class Unrelenting_Force : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.LogError("Unrelenting Force not yet implemented");
        }
    }
}
