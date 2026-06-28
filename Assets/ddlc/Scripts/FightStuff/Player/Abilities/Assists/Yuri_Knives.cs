using RenCSharp.EXPERIMENTAL;
using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Yuri_Knives : Player_Ability
    {
        [SerializeField] private Animated_Image_Handler knivesFab;
        [SerializeField, Min(0)] private float dispellTime = 10;
        [SerializeField] private int ticksBeforeDispell = 10;
        [SerializeField] private Sprite[] animationFrames;
        [SerializeField] private float secondsPerFrame = 0.1f;
        [Header("Bodio")]
        [SerializeField] private AudioClip summonSound;
        [SerializeField] private AudioClip damageTick;
        [SerializeField, Range(0,1)] private float volume;
        [SerializeField] private float minPitch;
        [SerializeField] private float maxPitch;

        private Animated_Image_Handler curKnives;
        private float dmg, tickDMG;
        private Transform enemyTransform;

        public override void FireAbility()
        {
            if (!validToFire || PlayerTurn || curKnives != null) return;
            t = 0;
            Event_Bus.TryFireFloatEvent("PlayerAbilityCooldown", t);
            dmg = (float)Flag_Manager.GetFlag(associatedTag);
            Event_Bus.AddVoidEvent("EnemyDied", EnemyStinker);
            tickDMG = dmg / ticksBeforeDispell;
            curKnives = Object_Factory.SpawnObject(knivesFab.gameObject, "YuriDoT", enemyTransform).GetComponent<Animated_Image_Handler>();
            curKnives.ReceiveAnimationInformation(animationFrames, secondsPerFrame);
            validToFire = false;
            Audio_Manager.AM.Play2DSFX(summonSound, minPitch, maxPitch, volume);
            StartCoroutine(DamageOverTime());
        }

        private IEnumerator DamageOverTime() //for some reason stops when another ability is active?
        {
            float t = 0;
            float f = 0;
            float tickTime = dispellTime / (float)ticksBeforeDispell;

            while (t < dispellTime)
            {
                f += Time.deltaTime;
                t += Time.deltaTime;

                if(f > tickTime)
                {
                    f = 0;
                    
                    if(Event_Bus.TryFireDoubleObjEvent("EnemyTakeDamage", (object)tickDMG, (object)false))
                    {
                        Audio_Manager.AM.Play2DSFX(damageTick, minPitch, maxPitch, volume);
                    }
                    else
                    {
                        //enemy dead means that the stinking event can't happen, so get rid of DoT!
                        Object_Factory.RemoveObject("YuriDoT");
                        if (Event_Bus.TryGetVoidEvent("EnemyDied", out Action venter))
                        {
                            venter -= EnemyStinker;
                        }
                        yield break;
                    }
                }
               
                yield return null;
            }
            if (Event_Bus.TryGetVoidEvent("EnemyDied", out Action vent))
            {
                vent -= EnemyStinker;
            }
            Object_Factory.RemoveObject("YuriDoT");
            //get rid of obj
        }

        private void EnemyStinker()
        {
            Object_Factory.RemoveObject("YuriDoT");
            if (Event_Bus.TryGetVoidEvent("EnemyDied", out Action vent))
            {
                vent -= EnemyStinker;
            }
        }

        private void Start()
        {
            Event_Bus.AddSingleObjEvent("GetEnemyTransform", GetEnemyTransform);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
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
    }
}
