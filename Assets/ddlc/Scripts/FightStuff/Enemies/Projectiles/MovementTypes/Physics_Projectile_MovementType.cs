using RenCSharp.EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Physics_Projectile_MovementType : Projectile_MovementType
    {
        [SerializeField] private bool ignoreMass = true;

        public override void UpdateMoveDir(Vector3 v3, bool first = false)
        {
            projectileRigidbody.linearVelocity = Vector3.zero;
            projectileRigidbody.AddForce(v3, ignoreMass ? ForceMode.VelocityChange : ForceMode.Impulse);
        }

        public override void MovementBehavior()
        {
            if (projectileRigidbody.linearVelocity == Vector3.zero || !movementSetsRotation) return;
            projectileTransform.rotation = TrigHelper.GetQuaternion(projectileRigidbody.linearVelocity.normalized);
        }

        public override Vector2 GetPositionAtTime(float time, Vector2 initialDirection, Vector3 spawnPosition)
        {
            Vector2 s = new Vector2(spawnPosition.x, spawnPosition.y);
            //just uses straight line logic because genuinely, fuck doing math
            return s + (time * speed) * initialDirection;
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
