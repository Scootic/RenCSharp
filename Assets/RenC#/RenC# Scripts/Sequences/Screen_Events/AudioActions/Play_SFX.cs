using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RenCSharp.Sequences
{
    /// <summary>
    /// Play a sound effect. Kinda obvious.
    /// </summary>
    [Obsolete("Outdated, should use Play_SFXAsset instead to communicate with Save/Load.", false)]
    public class Play_SFX : Screen_Event, IDeprecatedReplaceable<Screen_Event>
    {
        [SerializeField] private AudioClip sfxToPlay;
        [SerializeField, Tooltip("Leave as Vec3.zero to be a 2D sfx")] protected Vector3 position = Vector3.zero;
        [SerializeField, Tooltip("Decides if the sound effect should loop")] protected bool loop = false;
        [SerializeField] protected bool stopOnScreenProgress = true;
        [SerializeField, Min(0), Tooltip("Decides how long a sfx should loop for, unused if loop is false. " +
            "Leave at 0 if you want it to be stopped manually. This will make it an ESFX." +
            "SFX will be automatically stopped by screen changing if duration is not 0.")]
        protected float loopDuration = 1f;
        [SerializeField, Range(0f, 1f)] protected float baseVolume = 1f;
        protected Coroutine stopLoopRoutine;
        protected bool is3D => position != Vector3.zero;
        protected bool environmental => loopDuration == 0;

        public AudioClip GetSFXToPlay => sfxToPlay;
        public Vector3 GetPosition => position;
        public bool GetLoop => loop;
        public bool GetStopOnScreenProgress => stopOnScreenProgress;
        public float GetLoopDuration => loopDuration;
        public float GetBaseVolume => baseVolume;

        public override void DoEvent()
        {
            if (!is3D) Audio_Manager.AM.Play2DSFX(sfxToPlay, 1f, 1f, baseVolume, environmental, loop);
            else Audio_Manager.AM.Play3DSFX(sfxToPlay, position, environmental, loop, baseVolume);

            if (loop && loopDuration > 0)
            {
                stopLoopRoutine = Script_Manager.SM.StartCoroutine(HandleLoopDuration());
            }
            if (stopOnScreenProgress) Script_Manager.ProgressScreenEvent += PanicStopSFX;
        }

        protected virtual IEnumerator HandleLoopDuration()
        {
            if (environmental) yield break;
            float t = 0;
            while(t < loopDuration)
            {
                t += Time.deltaTime;
                yield return null;
            }
            if (is3D) Audio_Manager.AM.Stop3DSFX(sfxToPlay);
            else Audio_Manager.AM.Stop2DSFX(sfxToPlay);
        }

        protected virtual void PanicStopSFX()
        {
            if(stopLoopRoutine != null) Script_Manager.SM.StopCoroutine(stopLoopRoutine);
            if (is3D) Audio_Manager.AM.Stop3DSFX(sfxToPlay);
            else Audio_Manager.AM.Stop2DSFX(sfxToPlay);
        }

        public Screen_Event Replacement()
        {
            Play_SFXAsset newVentPS = new Play_SFXAsset();

            newVentPS.SetBaseVolume = baseVolume;
            newVentPS.SetLoop = loop;
            newVentPS.SetStopOnScreenProgress = stopOnScreenProgress;
            newVentPS.SetPosition = position;
            newVentPS.SetLoopDuration = loopDuration;

            string johnsonPS = AssetDatabase.GetAssetPath(sfxToPlay);
            string guidPS = AssetDatabase.AssetPathToGUID(johnsonPS);
            AssetReference stinky = new AssetReference(guidPS);
            newVentPS.SetAssetReference = stinky;

            return newVentPS;
        }

        public override string ToString()
        {
            return "Deprecated/Play Sound Effect";
        }
    }
}
