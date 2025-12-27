using UnityEngine;

namespace RenCSharp.Combat
{
    public class Base_Projectile : MonoBehaviour
    {
        [SerializeField] protected bool damageOverTime = false, destroyOnHit = false;
        [SerializeField, Min(0.1f)] protected float baseDamage = 1;
        [SerializeField, Min(0.1f)] protected float moveSpeed = 1;
        protected IDamage receiver;
        protected Vector3 moveDir;
        /// <summary>
        /// Sets the move direction that's used in update to change the projectile's position.
        /// </summary>
        /// <param name="v3">Something, probably player position</param>
        public virtual void UpdateMoveDir(Vector3 v3)
        {
            moveDir = v3;
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
            if (receiver == null) return;
            if (!damageOverTime) receiver.TakeDamage(baseDamage, false);
            if (destroyOnHit) Destroy(gameObject);
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
    }
}
