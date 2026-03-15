using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Explode_Projectile_DespawnType : Projectile_DespawnType
    {
        [SerializeField] private Vector3[] explosionDirs;
        [SerializeField] private Base_Projectile subProjectile;
        [SerializeField, Tooltip("Makes the explosionDirs apply in local space if true.")] private bool localExplosionDirections = false;
        public override void OnDespawn(bool playerTurn, Transform despawningTransform)
        {
            if (playerTurn) return; //don't shoot out projectiles if the turn is done. causes all sorts of unpleasant bs otherwise.
            foreach (Vector3 v in explosionDirs)
            {
                Base_Projectile bp = Object_Pooling.Spawn(subProjectile.gameObject, despawningTransform.position, Quaternion.identity).GetComponent<Base_Projectile>();
                bp.transform.SetParent(despawningTransform.parent);
                Event_Bus.TryFireSingleObjEvent("AddAProjectile", (object)bp.gameObject);
                Vector3 soundSpawnPos = Camera.main.transform.position + bp.transform.localPosition.normalized;
                Audio_Manager.AM.Play3DSFX(bp.SpawnSound, soundSpawnPos, false, false, bp.SpawnSoundVol, 0.9f, 1.1f);
                if (!localExplosionDirections)
                {
                    bp.UpdateMoveDir(v);
                }
                else
                {
                    Vector3 local = v.x * despawningTransform.right + v.y * despawningTransform.up;
                    bp.UpdateMoveDir(local);
                }
                bp.StartCoroutine(Object_Pooling.DespawnOverTime(bp.gameObject, bp.Lifetime));
            }
        }
        public override string ToString()
        {
            return "Explosion";
        }

        public override void OnEditorValidate()
        {
            return;
        }
    }
}
