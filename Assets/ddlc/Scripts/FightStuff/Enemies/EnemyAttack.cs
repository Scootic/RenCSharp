using UnityEngine;

namespace RenCSharp.Combat
{
    [CreateAssetMenu(menuName = "New Enemy Attack")]
    public class EnemyAttack : ScriptableObject
    {
        [SerializeField] private Vector2 arenaDimensions = new Vector2(600, 350);
        [SerializeReference] private ControlType controlType = new FreeRoam(); //default to freeroam, cause screw it
        [SerializeField, Min(1)] private float attackDuration = 5f;
        [SerializeField, Min(0)] private float secondsPerProjectileSpawn = 0.5f;
        [Header("flavor")]
        [SerializeField, TextArea(3, 20)] private string postAttackDescription;
        [Header("projectiles")]
        [SerializeField] private Base_Projectile[] projectilesThatSpawn;
        [SerializeField, Tooltip("Offset from arena center point")] private Vector3[] spawnPoints;
        [SerializeField, Tooltip("corresponds to spawn points. Please be normalized!")] private Vector3[] initialDirections;
        [SerializeField] private AttackSpawnSelectionMethod projectileSpawnMethod = AttackSpawnSelectionMethod.NoRepeatRandom;

        public AttackSpawnSelectionMethod ProjectileSpawnMethod => projectileSpawnMethod;
        public Vector2 ArenaDimensions => arenaDimensions;
        public ControlType ControlType => controlType;
        public Base_Projectile[] ProjectilesThatSpawn => projectilesThatSpawn;
        public Vector3[] InitialDirections => initialDirections;
        public Vector3[] SpawnPoints => spawnPoints;
        public float AttackDuration => attackDuration;
        public float SecondsPerProjectileSpawn => secondsPerProjectileSpawn;
        public string PostAttackDescription => postAttackDescription;
    }
}
