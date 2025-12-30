using UnityEngine;

namespace RenCSharp.Combat
{
    [CreateAssetMenu(menuName = "New Enemy")]
    public class EnemySO : ScriptableObject
    {
        [SerializeField] private EnemyAttack[] scriptedAttacks;
        [SerializeField] private EnemyAttack[] randomAttacks;
        [SerializeField, Min(1)] private int maxHealth = 10;
        [SerializeField] private Sprite[] visualInformation;
        [SerializeField, TextArea(3, 5)] private string defeatText, nameText;
        [SerializeField] private AudioClip hurtedSound;

        public EnemyAttack[] ScriptedAttacks => scriptedAttacks;
        public EnemyAttack[] RandomAttacks => randomAttacks;
        public int MaxHealth => maxHealth;
        public Sprite[] VisualInformation => visualInformation;
        public string NameText => nameText;
        public string DefeatText => defeatText;
        public AudioClip HurtedSound => hurtedSound;
    }
}
