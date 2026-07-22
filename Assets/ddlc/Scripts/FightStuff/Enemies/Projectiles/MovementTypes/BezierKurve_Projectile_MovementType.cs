using RenCSharp.EXPERIMENTAL;
using RenCSharp.Combat.Interfaces;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class BezierKurve_Projectile_MovementType : Projectile_MovementType
    {
        [SerializeField, Tooltip("How far the boomerang will go, along lifespan duration.")] private float distanceFromSpawn;
        [SerializeField] private float arcHeight = 300f;
        [SerializeField, Min(0), Tooltip("Should be the same as projectile lifetime, probably.")] private float travelDuration = 10;
        [SerializeField] private AnimationCurve animateBezCurve;
        [SerializeField] private BezierCurveType curveType = BezierCurveType.SimpleArc;

        private float eval;
        private float t;
        private Vector3[] boundingPositions = new Vector3[4];

        public override void UpdateMoveDir(Vector3 v3, bool first = false)
        {
            moveDir = v3;
            boundingPositions = BoundingBezierPositions.BoundingPositions4(curveType, projectileTransform.position, moveDir, distanceFromSpawn, arcHeight);
            t = 0;
        }

        public override void MovementBehavior()
        {
            t += Time.deltaTime;
            eval = t / travelDuration;
            Vector3 newPos = TrigHelper.BezPos(boundingPositions, eval);
            if (movementSetsRotation)
            {
                Vector3 prevPos = projectileTransform.position;
                Vector3 dirToFramePos = newPos - prevPos;
                if (dirToFramePos != Vector3.zero) projectileTransform.rotation = TrigHelper.GetQuaternion(dirToFramePos);
            }
            projectileTransform.position = newPos;
        }

        public override Vector2 GetPositionAtTime(float time, Vector2 initialDirection, Vector3 spawnPos, bool flipY = false)
        {
            if (flipY)
            {
                initialDirection = new Vector2(initialDirection.x, initialDirection.y * -1);
                spawnPos = new Vector3(spawnPos.x, spawnPos.y * -1);
            }

            Vector3 dir = new Vector3(initialDirection.x, initialDirection.y, 0);
            boundingPositions = BoundingBezierPositions.BoundingPositions4(curveType, spawnPos, dir, distanceFromSpawn, flipY ? arcHeight * -1 : arcHeight);
            eval = time / travelDuration;
            Vector3 pos = TrigHelper.BezPos(boundingPositions, eval);
            return new Vector2(pos.x, pos.y); 
        }

        public override string ToString()
        {
            return "Bezier Curve";
        }

        public override void OnEditorValidate()
        {
            return;
        }
    }
}
