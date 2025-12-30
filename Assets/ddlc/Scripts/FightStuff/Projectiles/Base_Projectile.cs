using System.Collections;
using UnityEngine;

namespace RenCSharp.Combat
{
    [RequireComponent(typeof(Collider))]
    public class Base_Projectile : MonoBehaviour, IDespawn
    {
        [Header("Base Projectile")]
        [SerializeField] protected bool damageOverTime = false;
        [SerializeField] protected bool destroyOnHit = true;
        [SerializeField, Min(0.1f)] protected float baseDamage = 1;
        [SerializeField, Min(0.1f)] protected float moveSpeed = 500;
        [SerializeField, Min(0.15f)] protected float lifetime = 10;
        [SerializeField, Range(0, 1)] protected float spawnSoundVol = 1;
        [SerializeField] protected AudioClip spawnSound;
        protected IDamage receiver;
        protected Vector3 moveDir;
        protected Collider myCol;
        public float Lifetime => lifetime;
        public float SpawnSoundVol => spawnSoundVol;
        public AudioClip SpawnSound => spawnSound;
        /// <summary>
        /// Sets the move direction that's used in update to change the projectile's position.
        /// </summary>
        /// <param name="v3">Something, probably player position</param>
        public virtual void UpdateMoveDir(Vector3 v3)
        {
            moveDir = v3;
            transform.rotation = VecToQuaternion.GetQuaternion(v3);
        }

        protected virtual void OnEnable()
        {
            myCol = GetComponent<Collider>();
            StartCoroutine(EnableTriggerOverTime());
        }

        protected IEnumerator EnableTriggerOverTime()
        {
            myCol.enabled = false;
            yield return new WaitForSeconds(0.1f);
            myCol.enabled = true;
        }

        //Handle movements
        protected virtual void Update()
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
        //Set up the procedures to take damage
        protected virtual void OnTriggerEnter(Collider other)
        {
            receiver = other.GetComponent<IDamage>();
            if (!damageOverTime && receiver != null) receiver.TakeDamage(baseDamage, false);
            if (destroyOnHit) 
            {
                Object_Pooling.Despawn(gameObject);
            }
        }
        //scale base damage down based on time.deltaTime since DoT is a per frame kind of thing.
        //basically turns baseDamage into baseDPS
        protected virtual void OnTriggerStay(Collider other)
        {
            if (!damageOverTime) return;
            if(receiver != null) receiver.TakeDamage(baseDamage * Time.deltaTime, true);
        }

        //hopefully no overlapping other triggers nonsense?
        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (receiver != null) receiver = null;
        }

        public virtual void OnDespawn()
        {

        }
    }
}
