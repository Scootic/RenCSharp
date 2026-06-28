using UnityEngine;
using RenCSharp.Combat.Interfaces;
using RenCSharp.EXPERIMENTAL;
namespace RenCSharp.Combat.Enemies
{
    public class Emitting_Projectile_CustomUpdate : Projectile_CustomUpdate
    {
        [SerializeField] private Vector3[] emissionDirs;
        [SerializeField, Min(1)] private int emitPerCycle = 1;
        [SerializeField, Min(0.01f)] private float timeToEmit = 0.25f;
        [SerializeField] private Base_Projectile projectileToEmit;
        [SerializeField, Tooltip("Makes emitted projectiles use local transform space")] private bool localEmissionDirs = true;
        [SerializeField] private AttackSpawnSelectionMethod runThroughEmissionDirs;
        [SerializeField] private Transform projectileTransform;

        private float t = 0;
        private int emIndex = 0;
        private int dir = 1;

        public override void UpdateBehavior()
        {
            t += Time.deltaTime;
            if(t >= timeToEmit)
            {
                t = 0;

                for(int i = 0; i < emitPerCycle; i++)
                {
                    if (emIndex == 0) dir = 1;
                    else if (emIndex >= emissionDirs.Length - 1) dir = -1;

                    int randI = runThroughEmissionDirs switch
                    {
                        AttackSpawnSelectionMethod.TrueRandom => Random.Range(0, emissionDirs.Length),
                        AttackSpawnSelectionMethod.NoRepeatRandom => RandomHelper.NoRepeatRoll("attackSpawnRoll", emissionDirs.Length - 1),
                        AttackSpawnSelectionMethod.LoopThrough => (emIndex >= emissionDirs.Length - 1) ? 0 : emIndex + 1,
                        AttackSpawnSelectionMethod.ReverseLoopThrough => (emIndex <= 0) ? emissionDirs.Length - 1 : emIndex - 1,
                        AttackSpawnSelectionMethod.PingPong => emIndex += dir,
                        _ => 0
                    };

                    emIndex = randI;
                    Vector3 v = emissionDirs[emIndex];

                    Base_Projectile bp = Object_Pooling.Spawn(projectileToEmit.gameObject, projectileTransform.position, Quaternion.identity).GetComponent<Base_Projectile>();
                    bp.transform.SetParent(projectileTransform.parent);
                    Event_Bus.TryFireSingleObjEvent("AddAProjectile", bp.gameObject);
                    Vector3 soundSpawnPos = Camera.main.transform.position + bp.transform.localPosition.normalized;
                    Audio_Manager.AM.Play3DSFX(bp.SpawnSound, soundSpawnPos, false, false, bp.SpawnSoundVol, 0.9f, 1.1f);

                    if (!localEmissionDirs)
                    {
                        bp.UpdateMoveDir(v);
                    }
                    else
                    {
                        Vector3 local = v.x * projectileTransform.right + v.y * projectileTransform.up;
                        bp.UpdateMoveDir(local);
                    }
                    bp.StartCoroutine(Object_Pooling.DespawnOverTime(bp.gameObject, bp.Lifetime));
                }
            }
        }

        public override string ToString()
        {
            return "Sub-projectile Emitter";
        }

        public override void OnEditorValidate()
        {
            return;
        }

        public override void OnEnable()
        {
            return;
        }

        public override void OnRemove(bool playerTurn)
        {
            t = 0;
        }
    }
}
