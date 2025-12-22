using UnityEngine;

namespace RenCSharp
{
    public class BGM_On_Start : MonoBehaviour
    {
        [SerializeField] private AudioClip bgm;
        [SerializeField, Min(0)] private float fadeTime = 3;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Audio_Manager.AM.PlayBGM(bgm, fadeTime);
        }
    }
}
