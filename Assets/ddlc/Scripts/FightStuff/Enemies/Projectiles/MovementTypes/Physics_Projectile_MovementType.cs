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

        public override Vector2 GetPositionAtTime(float time, Vector2 initialDirection, Vector3 spawnPosition, out Vector2 dirAtTime, bool flipY = false)
        {
            Vector2 s = new(spawnPosition.x, spawnPosition.y);
            if (flipY) { s = new Vector2(s.x, s.y * -1); initialDirection = new Vector2(initialDirection.x, initialDirection.y * -1); }
            initialDirection = initialDirection.normalized * speed;
            //assuming speed is ~500? very dangerous!
            Vector2 accel = initialDirection + new Vector2(0, flipY ? speed : -speed);
            float timeSqr = time * time;
            Vector2 posToReturn = s + initialDirection * time + (0.5f * timeSqr * accel);
            dirAtTime = (posToReturn - s).normalized;
            return posToReturn;
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
