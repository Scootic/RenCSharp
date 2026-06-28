using RenCSharp.EXPERIMENTAL;
using UnityEngine;
using System;
using System.Collections;
namespace RenCSharp.Combat.Player
{
    public class Monika_Sneak : Player_Ability
    {
        [Header("AnimInfo")]
        [SerializeField] private Sprite[] animFrames;
        [SerializeField] private float animationDuration;
        [SerializeField] private int hitTargetFrameIndex;
        [SerializeField] private Animated_Image_Handler shootFab;
        [Header("Audiooo")]
        [SerializeField] private float minPitch;
        [SerializeField] private float maxPitch;
        [SerializeField] private AudioClip shootSound;
        [SerializeField] private AudioClip hitSound;

        private Animated_Image_Handler curShoot;
        private bool shooting = false;
        private Action lambDelegate;
        private float dmg;
        private Transform enemyTransform;

        private void Start()
        {
            Event_Bus.AddSingleObjEvent("GetEnemyTransform", GetEnemyTransform);
        }

        private void OnDisable()
        {
            if (Event_Bus.TryGetSingleObjEvent("GetEnemyTransform", out Action<object> vent))
            {
                vent -= GetEnemyTransform;
            }
        }

        private void GetEnemyTransform(object transform)
        {
            enemyTransform = (Transform)transform;
        }

        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.Log("Monika Sneak Attack Fired!");
            dmg = (float)Flag_Manager.GetFlag(associatedTag);
            if(Object_Factory.TryGetObject("EnemyObject", out GameObject go))
            {
                curShoot = Object_Factory.SpawnObject(shootFab.gameObject, "MonikaSneakShot", enemyTransform).GetComponent<Animated_Image_Handler>();
                curShoot.ReceiveAnimationInformation(animFrames, animationDuration / (float)animFrames.Length);
                validToFire = false;
                shooting = true;
                lambDelegate = PanicStop;
                Player_Object.BeforeDisablePlayerAction += lambDelegate;
                Audio_Manager.AM.Play2DSFX(shootSound, minPitch, maxPitch);
                StartCoroutine(WaitToShoot());
            }
        }

        private void PanicStop()
        {
            if (!shooting) return;
            Event_Bus.TryFireDoubleObjEvent("EnemyTakeDamage", (object)dmg, (object)false);
            Audio_Manager.AM.Play2DSFX(hitSound, minPitch, maxPitch);
            Object_Factory.RemoveObject("MonikaSneakShot");
            Player_Object.BeforeDisablePlayerAction -= lambDelegate;
        }

        private IEnumerator WaitToShoot()
        {
            AbilityHelper.AnimationTiming(hitTargetFrameIndex, animFrames.Length, animationDuration, out float midPoint, out float remainder);
            yield return new WaitForSeconds(midPoint);
            if (!Event_Bus.TryFireDoubleObjEvent("EnemyTakeDamage", (object)dmg, (object)false)) 
            { 
                Player_Object.BeforeDisablePlayerAction -= lambDelegate;
                Object_Factory.RemoveObject("MonikaSneakShot");
                yield break; 
            }
            Audio_Manager.AM.Play2DSFX(hitSound, minPitch, maxPitch);
            yield return new WaitForSeconds(remainder);
            Object_Factory.RemoveObject("MonikaSneakShot");
            shooting = false;
            Player_Object.BeforeDisablePlayerAction -= lambDelegate;
        }
    }
}
