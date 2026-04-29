using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    /// <summary>
    /// Please only use this with physicers
    /// </summary>
    public class Bounce_Projectile_OnHitEffect : Projectile_OnHitEffect
    {
        [SerializeField] private Transform projectileTransform;
        [SerializeField] private Rigidbody projectileRigidbody;
        [SerializeField] private Base_Projectile projectile;
        [SerializeField] private float maxSpeed = 700f;
        [SerializeField, Min(0)] private float reboundStrength = 2;

        public override void OnHit(Collider other)
        {
            if (!validOnHit) return;
            validOnHit = false;
            Vector3 dirToOther = other.ClosestPoint(projectileTransform.position) - projectileTransform.position;
            dirToOther.Normalize();
            Vector3 thogcross = Vector3.Cross(dirToOther, Vector3.forward);
            Vector3 reflection = Vector3.Reflect(projectileRigidbody.linearVelocity, thogcross);
            reflection *= -1;
            
            Vector3 updatedForce = TrigHelper.ClampVector(reflection * reboundStrength, maxSpeed);
            projectile.UpdateMoveDir(updatedForce);
            Debug.Log("Phys Bounced, should set dir to: " + updatedForce);
            projectile.StartCoroutine(HandleCooldown());
        }

        public override string ToString()
        {
            return "Physics Bounce (please only use with physics movement type)";
        }
        public override void OnEditorValidate()
        {
            return;
        }
    }
}
