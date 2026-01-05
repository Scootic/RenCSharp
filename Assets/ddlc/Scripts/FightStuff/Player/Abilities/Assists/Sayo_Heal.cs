using UnityEngine;

namespace RenCSharp.Combat
{
    public class Sayo_Heal : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            Debug.Log("Sayori Heal Fired!");
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            int skill = Flag_Manager.GetFlag(associatedTag);
            if(Object_Factory.TryGetObject("PlayerObject", out GameObject go))
            {
                go.GetComponent<Player_Object>().TakeDamage(-skill, false);
            }
        }
    }
}
