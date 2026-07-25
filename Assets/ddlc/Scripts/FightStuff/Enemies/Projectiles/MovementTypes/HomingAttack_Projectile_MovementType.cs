
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class HomingAttack_Projectile_MovementType : Projectile_MovementType
    {
        [Header("Homing Settings")]
        [SerializeField, Min(0), Tooltip("Ignored if careAboutAlignment is false.")] private float homingStrength = 1f;
        [SerializeField, Tooltip("Makes the projectile home best when it is already facing player. (Also, home worse when not.)" +
            "Set to false to avoid using dot prod math and instead home in perfectly.")] 
        private bool careAboutAlignment = true;
        private Transform playerTransform;

        public override void UpdateMoveDir(Vector3 v3, bool first = false)
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
            Vector3 t = dirToPlayer;
            if (careAboutAlignment)
            {
                float dot = Vector3.Dot(moveDir, dirToPlayer);
                t = Vector3.Lerp(moveDir, dirToPlayer, homingStrength * Time.deltaTime * Mathf.Abs(dot));
                t.Normalize();
            }
            UpdateMoveDir(t);
            projectileTransform.position += moveDir * Time.deltaTime * speed;
        }

        public override Vector2 GetPositionAtTime(float time, Vector2 initialDirection, Vector3 spawnPosition, out Vector2 dirAtTime, bool flipY = false)
        {
            Vector2 s = new Vector2(spawnPosition.x, spawnPosition.y);
            if (flipY)
            {
                s = new Vector2(s.x, s.y * -1);
                initialDirection = new Vector2(initialDirection.x, initialDirection.y * -1);
            }
            dirAtTime = initialDirection;
            //same as straight line, because nothing to home into?
            return s + (speed * time) * initialDirection;
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
