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
        [SerializeField, Min(0)] private float reboundStrength = 2;
        public override void OnHit(Collider other)
        {
            Vector3 dirToOther = other.transform.position - projectileTransform.position;
            dirToOther.Normalize();
            Physics.Raycast(projectileTransform.position, dirToOther, out RaycastHit shit, 5f);
            Vector3 reflection = Vector3.Reflect(projectileRigidbody.linearVelocity, shit.normal);
            reflection *= -1;
            projectile.UpdateMoveDir(reflection * reboundStrength);
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
