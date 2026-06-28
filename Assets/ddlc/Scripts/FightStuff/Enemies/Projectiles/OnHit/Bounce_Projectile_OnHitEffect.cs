using RenCSharp.EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    /// <summary>
    /// Makes the projectile reflect. Very nice!
    /// </summary>
    public class Bounce_Projectile_OnHitEffect : Projectile_OnHitEffect
    {
        [SerializeField] private Transform projectileTransform;
        [SerializeField, Tooltip("Leave as null if the movement type isn't physics.")] private Rigidbody projectileRigidbody;
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
            Vector3 reflection;

            if (projectileRigidbody != null) //assume if we give the rigidbody that the movement type is physics, which should scale
            {
                reflection = Vector3.Reflect(projectileRigidbody.linearVelocity, thogcross);
                reflection *= -1;
                Vector3 updatedForce = TrigHelper.ClampVector(reflection * reboundStrength, maxSpeed);
                projectile.UpdateMoveDir(updatedForce);
            }
            else //otherwise, assume we're just straightline or sumthing, update the movedir to another normalized value.
            {
                reflection = Vector3.Reflect(projectile.GetMoveDir, thogcross);
                reflection *= -1;
                projectile.UpdateMoveDir(reflection);
            }

            projectile.StartCoroutine(HandleCooldown());
        }

        public override string ToString()
        {
            return "Bounce";
        }
        public override void OnEditorValidate()
        {
            return;
        }
    }
}
