using UnityEngine;

namespace RenCSharp.Combat
{
    public class Monika_Sneak : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            int dmg = Flag_Manager.GetFlag("MonikaDamageSkill", false);
            if(Object_Factory.TryGetObject("EnemyObject", out GameObject go))
            {
                go.GetComponent<EnemyObject>().TakeDamage(dmg, false);
            }
        }

    }
}
