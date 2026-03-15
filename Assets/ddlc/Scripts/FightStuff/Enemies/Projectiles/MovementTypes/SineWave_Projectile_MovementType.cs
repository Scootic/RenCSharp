using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class SineWave_Projectile_MovementType : Projectile_MovementType
    {
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float amplitude = 2f;
        private Vector3 ogMoveDir;
        private Vector3 parallelDir;

        public override void UpdateMoveDir(Vector3 v3)
        {
            projectileTransform.rotation = TrigHelper.GetQuaternion(v3);
            ogMoveDir = v3;
            parallelDir = Vector3.Cross(ogMoveDir, Vector3.forward);
        }

        public override void MovementBehavior()
        {
            moveDir = ogMoveDir + amplitude * Mathf.Sin(frequency * Time.time) * parallelDir;
            projectileTransform.rotation = TrigHelper.GetQuaternion(moveDir);
            projectileTransform.position += moveDir * speed * Time.deltaTime;
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
