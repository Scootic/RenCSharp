using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Sinewave_Projectile : Base_Projectile
    {
        [Header("Sine")]
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float amplitude = 2f;
        private Vector3 ogMoveDir;
        private Vector3 parallelDir;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void UpdateMoveDir(Vector3 v3)
        {
            transform.rotation = TrigHelper.GetQuaternion(v3);
            ogMoveDir = v3;
            parallelDir = Vector3.Cross(ogMoveDir, Vector3.forward);
        }

        protected override void Update()
        {
            //set moveDir

            moveDir = ogMoveDir + amplitude * Mathf.Sin(frequency * Time.time) * parallelDir;
            transform.rotation = TrigHelper.GetQuaternion(moveDir);
            base.Update();
        }
    }
}
