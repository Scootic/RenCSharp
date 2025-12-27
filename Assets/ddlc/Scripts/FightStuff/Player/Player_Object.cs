using UnityEngine;

namespace RenCSharp.Combat
{
    [RequireComponent(typeof(Player_Input))]
    public class Player_Object : MonoBehaviour, IDamage
    {
        [SerializeField, Min(1)] private int maxHealth = 20;
        private float curHealth;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            curHealth = maxHealth;
        }

        // Update is called once per frame
        //void Update()
        //{
        
        //}

        public void TakeDamage(float f)
        {
            curHealth -= f - (f * Resistance());

            if(curHealth <= 0)
            {
                //Game Over stuff here!
            }
        }

        public float Resistance()
        {
            return 0;
        }
    }
}
