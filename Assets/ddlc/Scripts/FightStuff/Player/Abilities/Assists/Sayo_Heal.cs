using UnityEngine;

namespace RenCSharp.Combat
{
    public class Sayo_Heal : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            int skill = Flag_Manager.GetFlag("SayoriHealSkill");
            if(Object_Factory.TryGetObject("PlayerObject", out GameObject go))
            {
                go.GetComponent<Player_Object>().TakeDamage(-skill, false);
            }
        }
    }
}
