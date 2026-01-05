using UnityEngine;

namespace RenCSharp.Combat
{
    public class Monika_Sneak : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.Log("Monika Sneak Attack Fired!");
            int dmg = Flag_Manager.GetFlag(associatedTag);
            if(Object_Factory.TryGetObject("EnemyObject", out GameObject go))
            {
                go.GetComponent<EnemyObject>().TakeDamage(dmg, false);
            }
        }

    }
}
