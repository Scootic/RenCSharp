using EXPERIMENTAL;
using RenCSharp.Combat.Interfaces;
using System.Collections;
using UnityEngine;
namespace RenCSharp.Combat.Enemies
{
    public class Emitting_Projectile : Base_Projectile
    {
        [Header("Emission")]
        [SerializeField] private Vector3[] emissionDirs;
        [SerializeField, Min(1)] private int emitPerCycle = 1;
        [SerializeField] private Base_Projectile projectileToEmit;
        [SerializeField, Min(0.01f), Tooltip("cyclical")] private float timeToEmit;
        [SerializeField, Tooltip("Makes emitted projectiles use local transform space for their dirs")] private bool localEmissionDirections = true;
        [SerializeField] private AttackSpawnSelectionMethod runThroughEmissionDirs;

        private float t = 0;
        private int emIndex = 0;
        private int dir = 1;
        protected override void OnEnable()
        {
            base.OnEnable();
            t = 0;
            emIndex = 0;
            StartCoroutine(EmitOverTime());
        }

        private IEnumerator EmitOverTime() //would probably get stopped by being pooled anyway
        {
            while (gameObject.activeInHierarchy)
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
                            AttackSpawnSelectionMethod.NoRepeatRandom => RandomHelper.NoRepeatRoll("attackSpawnRoll", emissionDirs.Length),
                            AttackSpawnSelectionMethod.LoopThrough => (emIndex >= emissionDirs.Length - 1) ? 0 : emIndex + 1,
                            AttackSpawnSelectionMethod.ReverseLoopThrough => (emIndex <= 0) ? emissionDirs.Length - 1 : emIndex - 1,
                            AttackSpawnSelectionMethod.PingPong => emIndex += dir,
                            _ => 0 //default scenario of garbage null enum, just return 0 and probably complain too
                        };

                        emIndex = randI;
                        Vector3 v = emissionDirs[emIndex];

                        Base_Projectile bp = Object_Pooling.Spawn(projectileToEmit.gameObject, transform.position, Quaternion.identity).GetComponent<Base_Projectile>();
                        bp.transform.SetParent(transform.parent);
                        Event_Bus.TryFireSingleObjEvent("AddAProjectile", (object)bp.gameObject);
                        Vector3 soundSpawnPos = Camera.main.transform.position + bp.transform.localPosition.normalized;
                        Audio_Manager.AM.Play3DSFX(bp.SpawnSound, soundSpawnPos, false, false, bp.SpawnSoundVol, 0.9f, 1.1f);

                        if (!localEmissionDirections)
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

                yield return null;
            }
        }
    }
}
