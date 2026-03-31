using EXPERIMENTAL;
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
