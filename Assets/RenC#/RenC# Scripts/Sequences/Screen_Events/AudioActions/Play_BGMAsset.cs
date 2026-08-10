using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RenCSharp.Sequences
{
    public class Play_BGMAsset : Screen_Event, INullAssetReferenceCheck
    {
        [SerializeField] protected AssetReference songAsset;
        [SerializeField, Min(0), Tooltip("0 for no fade out.")] protected float fadeTime = 1f;
        [SerializeField, Tooltip("Sets the new audio track to start at the current one's current duration.")] protected bool setToSameTime = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void DoEvent()
        {
            Debug.Log("Asset Playing Audio!");
            _ = Audio_Manager.AM.PlayBGM(songAsset, fadeTime, true, setToSameTime);
        }

        public override string ToString()
        {
            return "Audio/Play Music Track";
        }

        public bool HasNullAssetReferences()
        {
            return songAsset == null;
        }

        public AssetReference SetSongAsset { set { songAsset = value; } }

        public float SetFadeTime { set { fadeTime = value; } }
        public bool SetToSameTime { set { setToSameTime = value; } }
    }
}
