using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
namespace RenCSharp
{
    [CreateAssetMenu(menuName = "New Sprite Database")]
    public class Sprite_Database : ScriptableObject
    {
        [SerializeField] private List<Sprite> sprites;
        [SerializeField] private AssetReference myself;
        public List<Sprite> Sprites => sprites;
        public AssetReference Myself => myself;
    }
}
