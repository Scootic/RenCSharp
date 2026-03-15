
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class HomingAttack_Projectile_MovementType : Projectile_MovementType
    {
        [SerializeField, Min(0)] private float homingStrength = 1f;
        private Transform playerTransform;

        public override void UpdateMoveDir(Vector3 v3)
        {
            base.UpdateMoveDir(v3);
            if (playerTransform != null) return;
            if(Object_Factory.TryGetObject("PlayerObject", out GameObject go)) 
            {
                playerTransform = go.transform;
            }
        }

        public override void MovementBehavior()
        {
            Vector3 dirToPlayer = playerTransform.position - projectileTransform.position;
            dirToPlayer.Normalize();
            float dot = Vector3.Dot(moveDir, dirToPlayer);
            Vector3 t = Vector3.Lerp(moveDir, dirToPlayer, homingStrength * Time.deltaTime * Mathf.Abs(dot));
            t.Normalize();
            UpdateMoveDir(t);
            projectileTransform.position += moveDir * Time.deltaTime * speed;
        }

        public override string ToString()
        {
            return "Homing Projectile";
        }
        public override void OnEditorValidate()
        {
            return;
        }
    }
}
