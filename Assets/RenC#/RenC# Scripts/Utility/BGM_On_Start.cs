using UnityEngine;
using UnityEngine.AddressableAssets;
namespace RenCSharp
{
    public class BGM_On_Start : MonoBehaviour
    {
        [SerializeField] private AssetReference bgm;
        [SerializeField, Min(0)] private float fadeTime = 3;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _ = Audio_Manager.AM.PlayBGM(bgm, fadeTime);
        }
    }
}
