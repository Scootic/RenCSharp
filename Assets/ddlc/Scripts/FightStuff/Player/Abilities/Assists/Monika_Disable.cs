using UnityEngine;

namespace RenCSharp.Combat
{
    public class Monika_Disable: Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.Log("Monika Insta-kill fired!");
            if(Object_Factory.TryGetObject("EnemyObject", out GameObject go))
            {
                validToFire = false;
                go.GetComponent<EnemyObject>().TakeDamage(9999999, false);
            }
        }

    }
}
