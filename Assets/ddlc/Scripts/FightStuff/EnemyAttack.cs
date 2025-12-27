using UnityEngine;

namespace RenCSharp.Combat
{
    [CreateAssetMenu(menuName = "New Enemy Attack")]
    public class EnemyAttack : ScriptableObject
    {
        [SerializeField] private Rect arenaDimensions;
        [SerializeField] private ControlType controlType;
    }
}
