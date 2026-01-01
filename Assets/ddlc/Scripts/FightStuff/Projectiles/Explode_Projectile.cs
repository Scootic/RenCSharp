using UnityEngine;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat
{
    public class Explode_Projectile : Base_Projectile, IDespawn
    {
        [Header("Kaboom")]
        [SerializeField] private Vector3[] explosionDirs;
        [SerializeField] private Base_Projectile subProjectile;
        public override void OnDespawn()
        {
            if (Fight_Manager.FM.PlayerTurn) return; //don't do nonsense if turn is done
            foreach(Vector3 v in explosionDirs)
            {
                Base_Projectile bp = Object_Pooling.Spawn(subProjectile.gameObject, transform.position, Quaternion.identity).GetComponent<Base_Projectile>();
                bp.transform.SetParent(transform.parent);
                Fight_Manager.FM.AddProjectileToList(bp.gameObject);
                Vector3 soundSpawnPos = Camera.main.transform.position + bp.transform.localPosition.normalized;
                Audio_Manager.AM.Play3DSFX(bp.SpawnSound, soundSpawnPos, false, false, bp.SpawnSoundVol, 0.9f, 1.1f);
                bp.UpdateMoveDir(v);
            }
        }
    }
}
