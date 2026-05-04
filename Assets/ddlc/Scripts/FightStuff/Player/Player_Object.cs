using UnityEngine;
using System.Collections;
using EXPERIMENTAL;
using UnityEngine.UI;
using System;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat.Player
{
    [Serializable]
    public class Player_Object : MonoBehaviour, IDamage
    {
        [SerializeField, Min(1)] private int maxHealth = 20;
        [SerializeField] private float invincibilitySeconds = 0.25f;
        [SerializeField] private AudioSource hurtedSound;
        [SerializeField] private AnimationCurve invincibleCurve;
        [SerializeField] private bool main = false, debug = false;
        [SerializeField] private string playerResistanceFlag = "PlayerResistance";
        private bool invincible = false, hurtSoundGood = true;
        private float curHealth;
        [Range(0,1)]private float preResistance = 0, postResistance = 0;

        public static Action BeforeDisablePlayerAction;
        public float CurrentHealth => curHealth;
        public float Resistance() => postResistance;
        public Vector3 GetPosition => transform.position;

        public void StartOfFight()
        {
            Event_Bus.TryRemoveDoubleObjEvent("SetPlayerResistance");
            maxHealth = Flag_Manager.GetFlag("PlayerMaxHealth");
            invincible = false;
            curHealth = Flag_Manager.GetFlag("PlayerCurHealth");
            Event_Bus.AddDoubleObjEvent("SetPlayerResistance", SetNewResistance);
            preResistance = (float)Flag_Manager.GetFlag(playerResistanceFlag) / 100f; //important for future developments, but timing issue makes
            //defend option not work on the first turn. very strange.
            postResistance = preResistance;
            Event_Bus.TryFireFloatEvent("PlayerHealth", curHealth);
            Event_Bus.TryFireFloatEvent("PlayerHealthPerc", (curHealth / maxHealth));
        }

        public void ManualSetHealths(int newMax, float newcur)
        {
            maxHealth = newMax;
            curHealth = newcur;
        }

        /// <summary>
        /// Temporarily sets the player resistance.
        /// </summary>
        /// <param name="r">BOOL whether or not we reset to the initial value, or false for being a modifier</param>
        /// <param name="val">FLOAT the value that sets the new value if BOOL is false</param>
        private void SetNewResistance(object r, object val)
        {
            bool reset = (bool)r;
            float value = (float)val;
            if(reset) postResistance = preResistance;
            else
            {
                postResistance = preResistance + value;
            }
        }
        void OnEnable()
        {
            hurtSoundGood = true;
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
        public void TakeDamage(object floatarg, object boolarg)
        {
            float f = (float)floatarg;
            bool b = (bool)boolarg;
            if (invincible) return; //don't take damage if invincible. go figure!

            curHealth -= Mathf.Max(0.01f, f - (f * postResistance)); //makes sure that the player can't take negative damage, or just 0. cause that would be dumb
            //curHealth = Mathf.Max(curHealth, 0);
            curHealth = Mathf.Min(curHealth, maxHealth);

            if(debug)Debug.Log($"Damage Taken by {gameObject.name}: " + f);
            if (main)
            {
                Event_Bus.TryFireFloatEvent("PlayerHealth", Mathf.Max(curHealth, 0));
                Event_Bus.TryFireFloatEvent("PlayerHealthPerc", (curHealth / maxHealth));
            }

            if (curHealth <= 0)
            {
                if (main) //game over if a main player obj.
                {
                    Event_Bus.TryFireBoolEvent("EndAFight", true);
                }
                else
                {
                    Object_Factory.RemoveObject(gameObject.name);
                }
            }
            else if(hurtSoundGood && f > 0)
            {
                Audio_Manager.AM.Play2DSFX(hurtedSound.clip, 0.99f, 1.01f);
                hurtSoundGood = false;
                StartCoroutine(HurtSoundHandle());
            }

            if (!b && !invincible && f > 0) //only worry about IFrames if the damage is bulk, not over time
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

        
    }
}
