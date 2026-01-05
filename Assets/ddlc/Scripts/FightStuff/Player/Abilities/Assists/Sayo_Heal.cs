using UnityEngine;

namespace RenCSharp.Combat
{
    public class Sayo_Heal : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.Log("Sayori Heal Fired!");
            int skill = Flag_Manager.GetFlag(associatedTag);
            if(Object_Factory.TryGetObject("PlayerObject", out GameObject go))
            {
                validToFire = false;
                go.GetComponent<Player_Object>().TakeDamage(-skill, false);
            }
        }
    }
}
