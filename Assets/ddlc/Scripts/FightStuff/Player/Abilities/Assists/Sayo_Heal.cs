using UnityEngine;
using EXPERIMENTAL;
namespace RenCSharp.Combat.Player
{
    public class Sayo_Heal : Player_Ability
    {
        [SerializeField] private AudioClip healSound;
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.Log("Sayori Heal Fired!");
            float skill = (float)Flag_Manager.GetFlag(associatedTag);
            if(Object_Factory.TryGetObject("PlayerObject", out GameObject go))
            {
                Audio_Manager.AM.Play2DSFX(healSound, 0.95f, 1.05f, 1, false);
                validToFire = false;
                go.GetComponent<Player_Object>().TakeDamage((object)-skill, (object)false);
            }
        }
    }
}
