using UnityEngine;
using System.Collections.Generic;
using RenCSharp.Combat.Interfaces;
using RenCSharp.Combat.Player;
namespace RenCSharp.Combat.Enemies
{
    [CreateAssetMenu(menuName = "New Enemy Attack")]
    public class EnemyAttack : ScriptableObject
    {
        [SerializeField] private Vector2 arenaDimensions = new Vector2(600, 350);
        [SerializeReference] private ControlType controlType = new FreeRoam(); //default to freeroam, cause screw it
        [SerializeField, Min(1)] private float attackDuration = 5f;
        [SerializeField, Min(0.01f)] private float secondsPerProjectileSpawn = 0.5f;
        [Header("projectiles")]
        [SerializeField] private Base_Projectile[] projectilesThatSpawn;
        [SerializeField, Tooltip("Offset from arena center point (local position)")] private List<Vector3> spawnPoints = new();
        [SerializeField, Tooltip("Please be normalized (unless you are evil)!")] private List<Vector3> initialDirections = new();
        [SerializeField, Tooltip("chooses between the projectiles that spawn based on the projectile index method")] private List<int> indexes = new();
        [SerializeField] private AttackSpawnSelectionMethod projectileSpawnPositionMethod = AttackSpawnSelectionMethod.NoRepeatRandom;
        [SerializeField] private AttackSpawnSelectionMethod projectileIndexMethod = AttackSpawnSelectionMethod.NoRepeatRandom;

        //Automatically scale all of our arrays, so that we will never have a time in which we can get an index outside of one of them.
        private void OnValidate()
        {
            bool spDif = spawnPoints.Count != initialDirections.Count && spawnPoints.Count != indexes.Count;
            bool idDif = initialDirections.Count != spawnPoints.Count && initialDirections.Count != indexes.Count;
            bool iDif = indexes.Count != spawnPoints.Count && indexes.Count != initialDirections.Count;
            if (spDif || iDif || idDif)
            {
                int desLength = 0;

                if (spDif) desLength = spawnPoints.Count;
                if (idDif) desLength = initialDirections.Count;
                if (iDif) desLength = indexes.Count;

                while (spawnPoints.Count < desLength) spawnPoints.Add(Vector3.zero);
                while (initialDirections.Count < desLength) initialDirections.Add(Vector3.zero);
                while (indexes.Count < desLength) indexes.Add(0);

                while (spawnPoints.Count > desLength) spawnPoints.RemoveAt(spawnPoints.Count - 1);
                while (initialDirections.Count > desLength) initialDirections.RemoveAt(initialDirections.Count - 1);
                while (indexes.Count > desLength) indexes.RemoveAt(indexes.Count - 1);
            }
        }

        public AttackSpawnSelectionMethod ProjectileSpawnPositionMethod => projectileSpawnPositionMethod;
        public AttackSpawnSelectionMethod ProjectileIndexMethod => projectileIndexMethod;
        public Vector2 ArenaDimensions => arenaDimensions;
        public ControlType ControlType => controlType;
        public Base_Projectile[] ProjectilesThatSpawn => projectilesThatSpawn;
        public Vector3[] InitialDirections => initialDirections.ToArray();
        public Vector3[] SpawnPoints => spawnPoints.ToArray();
        public int[] Indexes => indexes.ToArray();
        public float AttackDuration => attackDuration;
        public float SecondsPerProjectileSpawn => secondsPerProjectileSpawn;
    }
}
