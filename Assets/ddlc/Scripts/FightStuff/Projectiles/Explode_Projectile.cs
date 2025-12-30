using UnityEngine;

namespace RenCSharp.Combat
{
    public class Explode_Projectile : Base_Projectile, IDespawn
    {
        [Header("Kaboom")]
        [SerializeField] private Vector3[] explosionDirs;
        [SerializeField] private Base_Projectile subProjectile;
        public override void OnDespawn()
        {
            foreach(Vector3 v in explosionDirs)
            {
                Vector3 spawnPos = v + transform.position;
                Base_Projectile bp = Object_Pooling.Spawn(subProjectile.gameObject, spawnPos, Quaternion.identity).GetComponent<Base_Projectile>();
                bp.transform.SetParent(transform.parent);
                Fight_Manager.FM.AddProjectileToList(bp.gameObject);
                bp.UpdateMoveDir(v);
            }
        }
    }
}
