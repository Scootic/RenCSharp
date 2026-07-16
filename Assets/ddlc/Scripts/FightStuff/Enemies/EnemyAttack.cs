using RenCSharp.Combat.Interfaces;
using RenCSharp.Combat.Player;
using System.Collections.Generic;
using UnityEngine;
namespace RenCSharp.Combat.Enemies
{
    [CreateAssetMenu(menuName = "New Enemy Attack")]
    public class EnemyAttack : ScriptableObject
    {
        [SerializeField, Tooltip("How big the arena will be when enabled. (Sizes may vary based on how canvas is scaled.)")] protected Vector2 arenaDimensions = new Vector2(600, 350);
        [SerializeReference, Tooltip("Basically what sort of behavior is assigned to player movement keys. Polymorphic!")] protected ControlType controlType = new FreeRoam(); //default to freeroam, cause screw it
        [SerializeField, Min(1), Tooltip("Duration in seconds of how long the attack lasts.")] protected float attackDuration = 5f;
        [SerializeField, Min(0.01f), Tooltip("cooldown in seconds that elapses between a projectile(s) being spawned.")] private float secondsPerProjectileSpawn = 0.5f;
        [SerializeField, Min(1), Tooltip("How many indices we run through at a time. X projectiles per secondsPerSpawn")] private int projectilesPerSpawn = 1;
        [Header("projectiles")]
        [SerializeField, Tooltip("All the projectiles that can be spawned. If null, doesn't spawn a projectile OR interrupt the attack, just moves on to next.")] protected Base_Projectile[] projectilesThatSpawn;
        [SerializeField, Tooltip("Offset from arena center point (local position)")] private List<Vector3> spawnPoints = new();
        [SerializeField, Tooltip("Please be normalized (unless you are evil)!")] private List<Vector3> initialDirections = new();
        [SerializeField, Tooltip("chooses between the projectiles that spawn based on the projectile index method")] private List<int> indexes = new();
        [SerializeField, Tooltip("Determines how the Fight_Manager selects the next index to use.")] protected AttackSpawnSelectionMethod projectileSpawnPositionMethod = AttackSpawnSelectionMethod.NoRepeatRandom;
        [SerializeField] protected AttackSpawnSelectionMethod projectileIndexMethod = AttackSpawnSelectionMethod.NoRepeatRandom;

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

        public void OverrideSpawnPoints(List<Vector3> newSP, List<Vector3> newID)
        {
            spawnPoints = newSP;
            initialDirections = newID;
            indexes = new();
            for(int i = 0; i < spawnPoints.Count; i++)
            {
                indexes.Add(0);
            }
        }

        public AttackSpawnSelectionMethod ProjectileSpawnPositionMethod => projectileSpawnPositionMethod;
        public AttackSpawnSelectionMethod SetProjectileSpawnPositionMethod { set { projectileSpawnPositionMethod = value; } }
        public AttackSpawnSelectionMethod ProjectileIndexMethod => projectileIndexMethod;
        public AttackSpawnSelectionMethod SetProjectileIndexMethod { set { projectileIndexMethod = value; } }
        public Vector2 ArenaDimensions => arenaDimensions;
        public Vector2 SetArenaDimensions { set { arenaDimensions = value; } }
        public ControlType ControlType => controlType;
        public ControlType SetControlType { set { controlType = value; } }
        public Base_Projectile[] ProjectilesThatSpawn => projectilesThatSpawn;
        public Base_Projectile[] SetProjectilesThatSpawn { set { projectilesThatSpawn = value; } }
        public Vector3[] InitialDirections => initialDirections.ToArray();
        public Vector3[] SpawnPoints => spawnPoints.ToArray();
        public int[] Indexes => indexes.ToArray();
        public float AttackDuration => attackDuration;
        public float SetAttackDuration { set { attackDuration = value; } }
        public float SecondsPerProjectileSpawn => secondsPerProjectileSpawn;
        public int ProjectilesPerSpawn => projectilesPerSpawn;
    }
}
