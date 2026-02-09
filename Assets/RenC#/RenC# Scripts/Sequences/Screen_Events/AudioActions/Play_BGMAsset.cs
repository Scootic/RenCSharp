using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RenCSharp.Sequences
{
    public class Play_BGMAsset : Screen_Event
    {
        [SerializeField] private AssetReference songAsset;
        [SerializeField, Min(0), Tooltip("0 for no fade out.")] private float fadeTime = 1f;
        [SerializeField, Tooltip("Sets the new audio track to start at the current one's current duration.")] private bool setToSameTime = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void DoEvent()
        {
            Debug.Log("Asset Playing Audio!");
            Audio_Manager.AM.PlayBGM(songAsset, fadeTime, true, setToSameTime);
        }

        public override string ToString()
        {
            return "Play Music Track";
        }
        public AssetReference SetSongAsset { set { songAsset = value; } }

        public float SetFadeTime { set { fadeTime = value; } }
        public bool SetToSameTime { set { setToSameTime = value; } }
    }
}
