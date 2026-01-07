using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Monika_Sneak : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.Log("Monika Sneak Attack Fired!");
            float dmg = (float)Flag_Manager.GetFlag(associatedTag);
            if(Object_Factory.TryGetObject("EnemyObject", out GameObject go))
            {
                validToFire = false;
                Event_Bus.TryFireDoubleObjEvent("EnemyTakeDamage", (object)dmg, (object)false);
            }
        }

    }
}
