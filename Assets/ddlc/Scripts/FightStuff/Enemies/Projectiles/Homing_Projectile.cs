using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Homing_Projectile : Base_Projectile
    {
        [Header("Homing BS")]
        [SerializeField, Min(0)] private float homeStrength = 1;
        private Transform playerTransform;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Object_Factory.TryGetObject("PlayerObject", out GameObject go))
            {
                playerTransform = go.transform;
            }
        }

        protected override void Update()
        {
            base.Update();
            Vector3 dirToPlayer = playerTransform.position - transform.position;
            dirToPlayer.Normalize();
            float dot = Vector3.Dot(moveDir, dirToPlayer);
            Vector3 t = Vector3.Lerp(moveDir, dirToPlayer, homeStrength * Time.deltaTime * Mathf.Abs(dot));
            UpdateMoveDir(t);
        }
    }
}
