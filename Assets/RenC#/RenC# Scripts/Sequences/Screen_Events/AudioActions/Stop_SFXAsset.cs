using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RenCSharp.Sequences
{
    public class Stop_SFXAsset : Stop_SFX
    {
        [SerializeField] private AssetReference sfxToStop;

        public AssetReference SetSFXToStop { set { sfxToStop = value; } }
        public bool SetIs3D { set { is3D = value; } }
        public override void DoEvent()
        {
            if (is3D) Audio_Manager.AM.Stop3DSFX(sfxToStop);
            else Audio_Manager.AM.Stop2DSFX(sfxToStop);
        }

        public override string ToString()
        {
            return "Audio/Stop SFX";
        }
    }
}
