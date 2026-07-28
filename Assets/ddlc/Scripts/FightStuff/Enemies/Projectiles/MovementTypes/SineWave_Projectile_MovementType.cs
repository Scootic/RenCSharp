using RenCSharp.EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class SineWave_Projectile_MovementType : Projectile_MovementType
    {
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float amplitude = 2f;

        private Vector3 initialPos;
        private Vector3 parallelDir;
        private float t;

        public override void UpdateMoveDir(Vector3 v3, bool first = false)
        {
            projectileTransform.rotation = TrigHelper.GetQuaternion(v3);
            moveDir = v3;
            parallelDir = Vector3.Cross(moveDir, Vector3.forward);
            initialPos = projectileTransform.position;
            t = 0;
        }

        public override void MovementBehavior()
        {
            t += Time.deltaTime;
            Vector3 newPos = initialPos + (speed * t * moveDir) + (speed * amplitude * Mathf.Sin(frequency * t) * parallelDir);
            if (movementSetsRotation) {
                Vector3 rotatoDir = newPos - projectileTransform.position;
                projectileTransform.rotation = TrigHelper.GetQuaternion(rotatoDir); 
            }
            projectileTransform.position = newPos;
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

            Vector3 posToReturn = spawnPosition + (speed * time * init) + (speed * amplitude * Mathf.Sin(frequency * time) * parallel);

            if (time == 0) 
            {
                dirAtTime = initialDir;
            }
            else
            {
                float prevTime = time - 0.01f;
                Vector3 slightlyOlderPosition = spawnPosition + (speed * prevTime * init) + (speed * amplitude * Mathf.Sin(frequency * prevTime) * parallel);
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
