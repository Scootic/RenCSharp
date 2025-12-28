using UnityEngine;

namespace RenCSharp.Combat
{
    [CreateAssetMenu(menuName = "New Enemy Attack")]
    public class EnemyAttack : ScriptableObject
    {
        [SerializeField] private Rect arenaDimensions;
        [SerializeField] private ControlType controlType;
        [SerializeField, Min(1)] private float attackDuration = 5f;
        [Header("flavor")]
        [SerializeField] private string postAttackDescription;
        [Header("projectiles")]
        [SerializeField] private Base_Projectile[] projectilesThatSpawn;
        [SerializeField, Tooltip("Offset from arena center point")] private Vector3[] spawnPoints;
        [SerializeField, Tooltip("corresponds to spawn points")] private Vector3[] initialDirections;

        public Rect ArenaDimensions => arenaDimensions;
        public ControlType ControlType => controlType;
        public Base_Projectile[] ProjectilesThatSpawn => projectilesThatSpawn;
        public Vector3[] InitialDirections => initialDirections;
        public Vector3[] SpawnPoints => spawnPoints;
        public float AttackDuration => attackDuration;
        public string PostAttackDescription => postAttackDescription;
    }
}
