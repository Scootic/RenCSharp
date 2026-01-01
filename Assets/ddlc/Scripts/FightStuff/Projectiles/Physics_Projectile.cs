using UnityEngine;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class Physics_Projectile : Base_Projectile
    {
        [Header("Physics")]
        [SerializeField] private byte bounces = 5;
        [SerializeField, Min(1)] private float reboundStrength = 1.5f;
        [SerializeField] private bool ignoreMass = true;
        private byte cBounce;
        private Rigidbody rb;
        protected override void OnEnable()
        {
            base.OnEnable();
            cBounce = 0;
            rb = GetComponent<Rigidbody>();
        }
        public override void UpdateMoveDir(Vector3 v3)
        {
            rb.AddForce(v3, ignoreMass ? ForceMode.VelocityChange : ForceMode.Impulse);
        }
        protected override void OnTriggerEnter(Collider other)
        {
            receiver = other.GetComponent<IDamage>();
            if (!damageOverTime && receiver != null) receiver.TakeDamage(baseDamage, false);
            cBounce++;
            if(cBounce <= bounces)
            {
                Vector3 dirToOther = other.transform.position - transform.position;
                dirToOther.Normalize();
                Physics.Raycast(transform.position, dirToOther, out RaycastHit shit, 5f);
                Vector3 reflection = TrigHelper.ReflectDirection(rb.linearVelocity, shit.normal);
                Debug.Log("Reflection: " + reflection);
                UpdateMoveDir(reflection * reboundStrength);
            }
            else if (destroyOnHit)
            {
                Object_Pooling.Despawn(gameObject);
            }
        }

        private void LateUpdate()
        {
            if (rb.linearVelocity == Vector3.zero) return;
            transform.rotation = TrigHelper.GetQuaternion(rb.linearVelocity.normalized);
        }
    }
}
