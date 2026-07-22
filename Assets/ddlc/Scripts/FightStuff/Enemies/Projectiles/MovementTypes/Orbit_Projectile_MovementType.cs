using RenCSharp.EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Orbit_Projectile_MovementType : Projectile_MovementType
    {
        [SerializeField, Tooltip("Determines radius of orbit circle.")] private float circleRadius = 400f;
        [SerializeField, Tooltip("Time in seconds it takes to complete a rotation.")] private float orbitTravelTime = 3f;
        [SerializeField, Tooltip("Determines if ts clockwise or nah. Kinda obvs.")] private bool clockwise = true;

        private float t = 0;
        private float eval;
        private Vector3 offset = Vector3.zero;
        public override void UpdateMoveDir(Vector3 v3, bool first = false)
        {
            t = clockwise ? orbitTravelTime : 0;
            base.UpdateMoveDir(v3);
        }

        public override void MovementBehavior()
        {
            t += clockwise ? -Time.deltaTime : Time.deltaTime;
            if (t > orbitTravelTime) t = 0;
            else if (t < 0) t = orbitTravelTime;
            eval = t / orbitTravelTime;

            offset += moveDir * speed * Time.deltaTime; //moves the center of the circle in the direction of the moveDir.

            Vector3 newPos = TrigHelper.PercentAlongUnitCirclePoint(eval,circleRadius) + offset;

            if (movementSetsRotation)
            {
                Vector3 oldPos = projectileTransform.localPosition;
                Vector3 dirToFramePos = newPos - oldPos;
                if(dirToFramePos != Vector3.zero) projectileTransform.rotation = TrigHelper.GetQuaternion(dirToFramePos);
            }

            projectileTransform.localPosition = newPos;
        }

        public override Vector2 GetPositionAtTime(float time, Vector2 initialDirection, Vector3 spawnPos, bool flipY = false)
        {
            if (flipY)
            {
                spawnPos = new Vector3(spawnPos.x, spawnPos.y * -1);
                initialDirection = new Vector2(initialDirection.x, initialDirection.y * -1);
            }

            Vector2 s = new Vector2(spawnPos.x, spawnPos.y);
            eval = time / orbitTravelTime;
            offset = (speed * time) * initialDirection;
            Vector3 newPos = TrigHelper.PercentAlongUnitCirclePoint(eval, circleRadius) + offset;
            return s + new Vector2(newPos.x, newPos.y);
        }

        public override string ToString()
        {
            return "Orbit";
        }

        public override void OnEditorValidate()
        {
            return;
        }
    }
}
