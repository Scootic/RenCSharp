using System.Collections;
using UnityEngine;

namespace RenCSharp.Sequences
{
    /// <summary>
    /// Play a sound effect. Kinda obvious.
    /// </summary>
    public class Play_SFX : Screen_Event
    {
        [SerializeField] private AudioClip sfxToPlay;
        [SerializeField, Tooltip("Leave as Vec3.zero to be a 2D sfx")] private Vector3 position = Vector3.zero;
        [SerializeField, Tooltip("Used for audio balancing")] private bool environmental = false;
        [SerializeField, Tooltip("Decides if the sound effect should loop")] private bool loop = false;
        [SerializeField, Min(0), Tooltip("Decides how long a sfx should loop for, unused if loop is false. " +
            "Leave at 0 if you want it to be stopped manually." +
            "SFX will be automatically stopped by screen changing if duration is not 0.")] private float loopDuration = 1f;
        private Coroutine stopLoopRoutine;
        private bool is3D => position != Vector3.zero;
        public override void DoShit()
        {
            if (!is3D) Audio_Manager.AM.Play2DSFX(sfxToPlay);
            else Audio_Manager.AM.Play3DSFX(sfxToPlay, position, environmental, loop);

            if (loop && loopDuration > 0)
            {
                stopLoopRoutine = Script_Manager.SM.StartCoroutine(HandleLoopDuration());
                Script_Manager.ProgressScreenEvent += PanicStopSFX;
            }
        }

        private IEnumerator HandleLoopDuration()
        {
            float t = 0;
            while(t < loopDuration)
            {
                t += Time.deltaTime;
                yield return null;
            }
            if (is3D) Audio_Manager.AM.Stop3DSFX(sfxToPlay);
            else Audio_Manager.AM.Stop2DSFX(sfxToPlay);
        }

        private void PanicStopSFX()
        {
            Script_Manager.SM.StopCoroutine(stopLoopRoutine);
            if (is3D) Audio_Manager.AM.Stop3DSFX(sfxToPlay);
            else Audio_Manager.AM.Stop2DSFX(sfxToPlay);
        }

        public override string ToString()
        {
            return "Play Sound Effect";
        }
    }
}
