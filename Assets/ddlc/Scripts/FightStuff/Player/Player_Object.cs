using UnityEngine;
using System.Collections;
namespace RenCSharp.Combat
{
    [RequireComponent(typeof(Player_Input))]
    public class Player_Object : MonoBehaviour, IDamage
    {
        [SerializeField, Min(1)] private int maxHealth = 20;
        private float curHealth;
        [SerializeField] private float invincibilitySeconds = 0.25f;
        private bool invincible = false;
  
        void Start()
        {
            invincible = false;
            curHealth = maxHealth;
        }

        private IEnumerator IFrames()
        {
            yield return new WaitForSeconds(invincibilitySeconds);
            invincible = false;
        }
        public void TakeDamage(float f, bool dot)
        {
            if (invincible) return; //don't take damage if invincible. go figure!

            curHealth -= f - (f * Resistance());

            if(curHealth <= 0)
            {
                //Game Over stuff here!
            }

            if (!dot) //only worry about IFrames if the damage is bulk, not over time
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
