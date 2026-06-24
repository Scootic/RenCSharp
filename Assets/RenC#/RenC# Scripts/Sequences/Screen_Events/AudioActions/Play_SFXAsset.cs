using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RenCSharp.Sequences
{
    public class Play_SFXAsset : Play_SFX
    {
        [SerializeField] private AssetReference sfxAsset;

        public AssetReference SetAssetReference { set { sfxAsset = value; } }
        public bool SetLoop { set {loop = value; } }
        public float SetBaseVolume { set { baseVolume = value; } }
        public Vector3 SetPosition { set { position = value; } }
        public bool SetStopOnScreenProgress { set  { stopOnScreenProgress = value; } }
        public float SetLoopDuration { set { loopDuration = value; } }
        public override void DoEvent()
        {
            if (!is3D) Audio_Manager.AM.Play2DSFX(sfxAsset, 1f, 1f, baseVolume, environmental, loop);
            else Audio_Manager.AM.Play3DSFX(sfxAsset, position, 1f, 1f, baseVolume, environmental, loop);

            if (loop && loopDuration > 0)
            {
                stopLoopRoutine = Script_Manager.SM.StartCoroutine(HandleLoopDuration());
            }
            if (stopOnScreenProgress) Script_Manager.ProgressScreenEvent += PanicStopSFX;
        }

        protected override IEnumerator HandleLoopDuration()
        {
            if(environmental) yield break;
            float t = 0;
            while(t < loopDuration)
            {
                t += Time.deltaTime;
                yield return null;
            }
            if (is3D) Audio_Manager.AM.Stop3DSFX(sfxAsset);
            else Audio_Manager.AM.Stop2DSFX(sfxAsset);
        }

        protected override void PanicStopSFX()
        {
            if (stopLoopRoutine != null) Script_Manager.SM.StopCoroutine(stopLoopRoutine);
            if (is3D) Audio_Manager.AM.Stop3DSFX(sfxAsset);
            else Audio_Manager.AM.Stop2DSFX(sfxAsset);
        }

        public override string ToString()
        {
            return "Audio/Play SFX";
        }
    }
}
