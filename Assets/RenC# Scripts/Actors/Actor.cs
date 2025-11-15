using UnityEngine;
using System;
namespace RenCSharp.Actors
{
    [Serializable]
    [CreateAssetMenu(menuName = "New Actor")]
    public class Actor : ScriptableObject
    {
        [SerializeField] private string actorName;
        [SerializeField] private GameObject actorPrefab;
        [SerializeField, Tooltip("Body Layer")] private Sprite[] poses;
        [SerializeField, Tooltip("Facial Layer")] private Sprite[] expressions;

        public string ActorName => actorName;
        public Sprite[] Poses => poses;
        public Sprite[] Expressions => expressions;
        public GameObject ActorPrefab => actorPrefab;
    }
}
