using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Sayo_Heal : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.Log("Sayori Heal Fired!");
            float skill = (float)Flag_Manager.GetFlag(associatedTag);
            if(Object_Factory.TryGetObject("PlayerObject", out GameObject go))
            {
                validToFire = false;
                go.GetComponent<Player_Object>().TakeDamage((object)-skill, (object)false);
            }
        }
    }
}
