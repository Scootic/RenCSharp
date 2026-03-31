using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Physics_Projectile_MovementType : Projectile_MovementType
    {
        [SerializeField] private bool ignoreMass = true;

        public override void UpdateMoveDir(Vector3 v3, bool first = false)
        {
            projectileRigidbody.AddForce(v3, ignoreMass ? ForceMode.VelocityChange : ForceMode.Impulse);
        }

        public override void MovementBehavior()
        {
            if (projectileRigidbody.linearVelocity == Vector3.zero || !movementSetsRotation) return;
            projectileTransform.rotation = TrigHelper.GetQuaternion(projectileRigidbody.linearVelocity.normalized);
        }

        public override string ToString()
        {
            return "Physics";
        }

        public override void OnEditorValidate()
        {
            return;
        }
    }
}
