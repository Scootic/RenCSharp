using RenCSharp.EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class SineWave_Projectile_MovementType : Projectile_MovementType
    {
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float amplitude = 2f;
        private Vector3 ogMoveDir;
        private Vector3 parallelDir;

        public override void UpdateMoveDir(Vector3 v3, bool first = false)
        {
            projectileTransform.rotation = TrigHelper.GetQuaternion(v3);
            ogMoveDir = v3;
            parallelDir = Vector3.Cross(ogMoveDir, Vector3.forward);
        }

        public override void MovementBehavior()
        {
            moveDir = ogMoveDir + amplitude * Mathf.Sin(frequency * Time.time) * parallelDir;
            if(movementSetsRotation) projectileTransform.rotation = TrigHelper.GetQuaternion(moveDir);
            projectileTransform.position += moveDir * speed * Time.deltaTime;
        }

        public override Vector2 GetPositionAtTime(float time, Vector2 initialDir, Vector3 spawnPosition, out Vector2 dirAtTime, bool flipY = false)
        {
            if (flipY)
            {
                initialDir = new Vector2(initialDir.x, initialDir.y * -1);
                spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y * -1);
            }
            Vector3 parallel = Vector3.Cross(initialDir, Vector3.forward);
            Vector3 init = new Vector3(initialDir.x, initialDir.y);

            Vector3 posToReturn = (spawnPosition + (init * speed * time)) + parallel * amplitude * Mathf.Sin(frequency * time);

            if (time == 0) 
            {
                dirAtTime = initialDir;
            }
            else
            {
                float prevTime = time - 0.01f;
                Vector3 slightlyOlderPosition = (spawnPosition + (init * speed * prevTime) + parallel * amplitude * Mathf.Sin(frequency * prevTime));
                Vector3 dirTypeShi = posToReturn - slightlyOlderPosition;
                dirAtTime = new Vector2(dirTypeShi.x, dirTypeShi.y);
            }

            return posToReturn;
        }

        public override string ToString()
        {
            return "Sine Wave";
        }
        public override void OnEditorValidate()
        {
            return;
        }
    }
}
