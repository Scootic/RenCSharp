using UnityEngine;

namespace RenCSharp.Combat
{
    public class Monika_Disable: Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if(Object_Factory.TryGetObject("EnemyObject", out GameObject go))
            {
                go.GetComponent<EnemyObject>().TakeDamage(9999999, false);
            }
        }

    }
}
