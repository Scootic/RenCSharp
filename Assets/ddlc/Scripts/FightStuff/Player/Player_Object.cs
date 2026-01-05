using UnityEngine;
using System.Collections;
using EXPERIMENTAL;
using UnityEngine.UI;
using System;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat
{
    [Serializable]
    public class Player_Object : MonoBehaviour, IDamage
    {
        [SerializeField, Min(1)] private int maxHealth = 20;
        [SerializeField] private float invincibilitySeconds = 0.25f;
        [SerializeField] private AudioSource hurtedSound;
        [SerializeField] private AnimationCurve invincibleCurve;
        private bool invincible = false, hurtSoundGood = true;
        private float curHealth, preResistance = 0, postResistance;

        public float CurrentHealth => curHealth;
        public void StartOfFight()
        {
            maxHealth = Flag_Manager.GetFlag("PlayerMaxHealth");
            invincible = false;
            curHealth = Flag_Manager.GetFlag("PlayerCurHealth");
            //preResistance = (float)Flag_Manager.GetFlag("PlayerResistance") / 100f; important for future developments, but timing issue makes
            //defend option not work on the first turn. very strange.
            //postResistance = preResistance;
            Event_Bus.TryFireFloatEvent("PlayerHealth", curHealth);
            Event_Bus.TryFireFloatEvent("PlayerHealthPerc", (curHealth / maxHealth));
        }

        public void SetNewResistance(bool reset, float val)
        {
            if(reset) postResistance = preResistance;
            else
            {
                postResistance = val;
            }
        }

        void OnDisable()
        {
            invincible = false;
        }
        private IEnumerator IFrames()
        {
            float t = 0;
            float eval;
            Image img = GetComponent<Image>();
            Color ogC = img.color;
            while(t <= invincibilitySeconds)
            {
                t += Time.deltaTime;
                eval = t / invincibilitySeconds;
                img.color = Color.Lerp(ogC, Color.white, invincibleCurve.Evaluate(eval));
                yield return null;
            }
            invincible = false;
        }
        public void TakeDamage(float f, bool dot)
        {
            if (invincible) return; //don't take damage if invincible. go figure!

            curHealth -= f - (f * Resistance());
            curHealth = Mathf.Max(curHealth, 0);
            curHealth = Mathf.Min(curHealth, maxHealth);

            Event_Bus.TryFireFloatEvent("PlayerHealth", curHealth);
            Event_Bus.TryFireFloatEvent("PlayerHealthPerc", (curHealth / maxHealth));

            if (curHealth == 0)
            {
                //Game Over stuff here!
                Fight_Manager.FM.EndAFight(true);
            }
            else if(hurtSoundGood)
            {
                Audio_Manager.AM.Play2DSFX(hurtedSound.clip, 0.99f, 1.01f);
                hurtSoundGood = false;
                StartCoroutine(HurtSoundHandle());
            }

            if (!dot && !invincible && f > 0) //only worry about IFrames if the damage is bulk, not over time
            {
                invincible = true;
                StartCoroutine(IFrames());
            }
        }

        private IEnumerator HurtSoundHandle()
        {
            yield return new WaitForSeconds(invincibilitySeconds - 0.01f);
            hurtSoundGood = true;
        }

        public float Resistance()
        {
            return postResistance;
        }

        public Vector3 GetPosition => transform.position;
    }
}
