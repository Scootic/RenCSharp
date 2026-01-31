using EXPERIMENTAL;
using System.Collections;
using UnityEngine;
using System;
namespace RenCSharp.Combat.Player
{
    public class Sayo_Heal : Player_Ability
    {
        [SerializeField] private Sprite[] healSprites;
        [SerializeField, Min(0)] private float healAnimationDuration;
        [SerializeField] private int healFrame;
        [SerializeField] private Animated_Image_Handler healFab;
        [Header("audo")]
        [SerializeField] private AudioClip healSound;
        [SerializeField] private AudioClip startHeal;
        [SerializeField] private float minPitch = 0.975f;
        [SerializeField] private float maxPitch = 1.025f;
        private Animated_Image_Handler curHeal;
        private bool healing = false;
        private float skill;
        private Action lambDelegate;
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.Log("Sayori Heal Fired!");
            skill = (float)Flag_Manager.GetFlag(associatedTag);
            if(Object_Factory.TryGetObject("PlayerObject", out GameObject go))
            {
                healing = true;
                lambDelegate = delegate { Panic(go.GetComponent<Player_Object>()); };
                Player_Object.BeforeDisablePlayerAction += lambDelegate;
                curHeal = Object_Factory.SpawnObject(healFab.gameObject, "HealAnim", go.transform).GetComponent<Animated_Image_Handler>();
                float SPF = healAnimationDuration / (float)healSprites.Length;
                curHeal.ReceiveAnimationInformation(healSprites, SPF);
                validToFire = false;
                Audio_Manager.AM.Play2DSFX(startHeal, minPitch, maxPitch);
                StartCoroutine(WaitToHeal(go));
            }
        }

        private void Panic(Player_Object player)
        {
            if (!healing) return;
            player.TakeDamage((object)-skill, (object)false);
            Audio_Manager.AM.Play2DSFX(healSound, minPitch, maxPitch, 1, false);
            Object_Factory.RemoveObject("HealAnim");
            healing = false;
            Player_Object.BeforeDisablePlayerAction -= lambDelegate;
        }

        private IEnumerator WaitToHeal(GameObject go)
        {
            AbilityHelper.AnimationTiming(healFrame, healSprites.Length, healAnimationDuration, out float percToHeal, out float remainder);
            yield return new WaitForSeconds(percToHeal);
            go.GetComponent<Player_Object>().TakeDamage((object)-skill, (object)false);
            Audio_Manager.AM.Play2DSFX(healSound, minPitch, maxPitch, 1, false);
            yield return new WaitForSeconds(remainder);
            Object_Factory.RemoveObject("HealAnim");
            Player_Object.BeforeDisablePlayerAction -= lambDelegate;
            healing = false;
        }
    }
}
