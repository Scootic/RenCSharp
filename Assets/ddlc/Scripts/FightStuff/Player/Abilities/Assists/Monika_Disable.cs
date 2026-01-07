using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Monika_Disable: Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.Log("Monika Insta-kill fired!");
            if(Object_Factory.TryGetObject("EnemyObject", out GameObject go))
            {
                validToFire = false;
                Event_Bus.TryFireDoubleObjEvent("EnemyTakeDamage", (object)999999f, (object)false);
            }
        }

    }
}
