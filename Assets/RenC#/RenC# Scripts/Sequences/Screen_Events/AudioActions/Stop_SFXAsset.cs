using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RenCSharp.Sequences
{
    /// <summary>
    /// The assumption is that if you're using this, you're trying to get rid of a sound effect that's looping forever.
    /// Play_SFXAsset should make it so that forever looping SFX are environmental, and are affected by ESFX settings.
    /// This stop assumes that the sfx you gave it is environmental.
    /// </summary>
    public class Stop_SFXAsset : Screen_Event
    {
        [SerializeField] private AssetReference sfxToStop;
        [SerializeField] private bool is3D = false;
        [Header("Fading")]
        [SerializeField] private bool fadeOut = true;

        public AssetReference SetSFXToStop { set { sfxToStop = value; } }
        public bool SetIs3D { set { is3D = value; } }
        public override void DoEvent()
        {
            if (is3D) _ = Audio_Manager.AM.Stop3DSFX(sfxToStop, true, fadeOut, true);
            else _ = Audio_Manager.AM.Stop2DSFX(sfxToStop, true, fadeOut, true);
        }

        public override string ToString()
        {
            return "Audio/Stop SFX";
        }
    }
}
