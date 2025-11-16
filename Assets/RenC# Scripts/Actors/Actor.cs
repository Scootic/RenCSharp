using UnityEngine;
using System;
namespace RenCSharp.Actors
{
    /// <summary>
    /// A scriptable object containing all the information that a character that appears will screen will need.
    /// A name, a specific prefab (to make sure sizing is to your liking), and sprites for poses and facial expressions.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "New Actor")]
    public class Actor : ScriptableObject
    {
        [SerializeField] private string actorName;
        [SerializeField] private GameObject actorPrefab;
        [SerializeField, Tooltip("2D Array. 0 is bottom layer, with each successive layer being on top of previous.")] private SpriteArray[] visuals;

        public string ActorName => actorName;
        public SpriteArray[] Visuals => visuals;
        public GameObject ActorPrefab => actorPrefab;
    }
}
