using UnityEngine;
using System.Collections;
using EXPERIMENTAL;
using UnityEngine.UI;
using System;
namespace RenCSharp.Combat
{
    [Serializable]
    public class Player_Object : MonoBehaviour, IDamage
    {
        [SerializeField, Min(1)] private int maxHealth = 20;
        private float curHealth;
        [SerializeField] private float invincibilitySeconds = 0.25f;
        [SerializeField] private AudioSource hurtedSound;
        [SerializeField] private AnimationCurve invincibleCurve;
        private bool invincible = false;
  
        public void StartOfFight()
        {
            invincible = false;
            curHealth = maxHealth;

            Event_Bus.TryFireFloatEvent("PlayerHealth", curHealth);
            Event_Bus.TryFireFloatEvent("PlayerHealthPerc", (curHealth / maxHealth));
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

            Event_Bus.TryFireFloatEvent("PlayerHealth", curHealth);
            Event_Bus.TryFireFloatEvent("PlayerHealthPerc", (curHealth / maxHealth));

            if (curHealth == 0)
            {
                //Game Over stuff here!
                Fight_Manager.FM.EndAFight(true);
            }
            else
            {
                Audio_Manager.AM.Play2DSFX(hurtedSound.clip, 0.99f, 1.01f);
            }

            if (!dot && !invincible) //only worry about IFrames if the damage is bulk, not over time
            {
                invincible = true;
                StartCoroutine(IFrames());
            }
        }

        public float Resistance()
        {
            return 0;
        }

        public Vector3 GetPosition => transform.position;
    }
}
