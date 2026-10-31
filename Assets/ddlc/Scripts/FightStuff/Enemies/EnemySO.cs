using UnityEngine;
using UnityEngine.AddressableAssets;
namespace RenCSharp.Combat.Enemies
{
    [CreateAssetMenu(menuName = "Enemies/New Enemy")]
    public class EnemySO : ScriptableObject
    {
        [Header("Scripted Attacks")]
        [SerializeField] private EnemyAttack[] scriptedAttacks;
        [SerializeField, TextArea(1, 3)] private string[] scriptedFlavorTexts;
        [Header("Random Attacks")]
        [SerializeField] private EnemyAttack[] randomAttacks;
        [SerializeField, TextArea(1,3)] private string[] randomFlavorTexts;
        [Header("Enemy Information/Stats")]
        [SerializeField, Min(1), Tooltip("The maximum amount of enemy health.")] private int maxHealth = 10;
        [SerializeField, Min(1), Tooltip("The health the enemy will start a fight with.")] private int startingHealth = 10;
        [SerializeField, Min(0), Tooltip("The health the fight will end with, default is zero (enemy is 'killed' not just defeated).")] private int endFightHealth = 0;
        [Header("Enemy Information/Cosmetics")]
        [SerializeField] private Sprite[] visualInformation;
        [SerializeField] private EnemyObject enemyPrefab;
        [SerializeField, TextArea(3, 5)] private string defeatText, nameText, blurbText;
        [SerializeField] private AudioClip hurtedSound;
        [SerializeField] private AssetReference uiParticleObjFab;
        [SerializeField] private AssetReference hitParticleObjFab;
        [SerializeField] private AssetReference deathParticleObjFab;

        private void OnValidate()
        {
            if (scriptedFlavorTexts.Length != scriptedAttacks.Length)
            {
                string[] newstr = new string[scriptedAttacks.Length];
                for (int i = 0; i < newstr.Length; i++) newstr[i] = i < scriptedFlavorTexts.Length ? scriptedFlavorTexts[i] : "null";
                scriptedFlavorTexts = newstr;
            }
            if (randomFlavorTexts.Length != randomAttacks.Length)
            {
                string[] newstr = new string[randomAttacks.Length];
                for (int i = 0; i < newstr.Length; i++) newstr[i] = i < randomFlavorTexts.Length ? randomFlavorTexts[i] : "null";
                randomFlavorTexts = newstr;
            }
        }

        public EnemyAttack[] ScriptedAttacks => scriptedAttacks;
        public string[] ScriptedFlavorTexts => scriptedFlavorTexts;
        public EnemyAttack[] RandomAttacks => randomAttacks;
        public string[] RandomFlavorTexts => randomFlavorTexts;
        public int MaxHealth => maxHealth;
        public int StartingHealth => startingHealth;
        public int EndFightHealth => endFightHealth;
        public Sprite[] VisualInformation => visualInformation;
        public string NameText => nameText;
        public string DefeatText => defeatText;
        public string BlurbText => blurbText;
        public string UIParticleGUID => uiParticleObjFab.AssetGUID;
        public string HitParticlePrefabGUID => hitParticleObjFab.AssetGUID;
        public string DeathParticlePrefabGUID => deathParticleObjFab.AssetGUID;
        public AudioClip HurtedSound => hurtedSound;
        public EnemyObject EnemyPrefab => enemyPrefab;
    }
}
