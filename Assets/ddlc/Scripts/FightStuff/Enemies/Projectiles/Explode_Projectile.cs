using UnityEngine;
using RenCSharp.Combat.Interfaces;
using EXPERIMENTAL;
namespace RenCSharp.Combat.Enemies
{
    public class Explode_Projectile : Base_Projectile, IDespawn
    {
        [Header("Kaboom")]
        [SerializeField] private Vector3[] explosionDirs;
        [SerializeField] private Base_Projectile subProjectile;
        [SerializeField, Tooltip("Makes the explosionDirs apply in local space rather than world space.")] private bool localExplosionDirections = false;
        public override void OnDespawn(bool playerTurn)
        {
            base.OnDespawn(playerTurn);
            if (playerTurn) return; //don't do nonsense if turn is done
            foreach (Vector3 v in explosionDirs)
            {
                Base_Projectile bp = Object_Pooling.Spawn(subProjectile.gameObject, transform.position, Quaternion.identity).GetComponent<Base_Projectile>();
                bp.transform.SetParent(transform.parent);
                Event_Bus.TryFireSingleObjEvent("AddAProjectile", (object)bp.gameObject);
                Vector3 soundSpawnPos = Camera.main.transform.position + bp.transform.localPosition.normalized;
                Audio_Manager.AM.Play3DSFX(bp.SpawnSound, soundSpawnPos, false, false, bp.SpawnSoundVol, 0.9f, 1.1f);
                if (!localExplosionDirections)
                {
                    bp.UpdateMoveDir(v);
                }
                else
                {
                    Vector3 local = v.x * transform.right + v.y * transform.up;
                    bp.UpdateMoveDir(local);
                }
                bp.StartCoroutine(Object_Pooling.DespawnOverTime(bp.gameObject, bp.Lifetime));
            }
        }
    }
}
