using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Homing_Projectile : Base_Projectile
    {
        [Header("Homing BS")]
        [SerializeField, Range(0,1)] private float homeStrength = 1;
        [SerializeField, Range(0,1)] private float distanceWeakener = 0.2f;
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
            float distanceFromPlayer = dirToPlayer.magnitude * distanceWeakener;
            dirToPlayer.Normalize();
            Vector3 t = Vector3.Lerp(moveDir, dirToPlayer, homeStrength * Time.deltaTime * distanceFromPlayer);
            UpdateMoveDir(t);
        }
    }
}
