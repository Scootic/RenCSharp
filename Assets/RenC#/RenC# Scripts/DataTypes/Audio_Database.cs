using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
namespace RenCSharp
{
    [CreateAssetMenu(menuName = "New Audio Database")]
    public class Audio_Database : ScriptableObject
    {
        [SerializeField] private List<AudioClip> sounds;
        [SerializeField] private AssetReference myself;
        public List<AudioClip> Sounds => sounds;
        public AssetReference Myself => myself;
    }
}
