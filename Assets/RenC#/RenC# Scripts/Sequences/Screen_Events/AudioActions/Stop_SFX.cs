using UnityEngine;

namespace RenCSharp.Sequences
{
    /// <summary>
    /// Stops a sound effect. Should only be used with SFX that you leave looping indefinetly to prevent chicaney.
    /// </summary>
    public class Stop_SFX : Screen_Event
    {
        [SerializeField] private AudioClip clipToStop;
        [SerializeField] private bool is3D = false;

        public override void DoShit()
        {
            if (is3D) Audio_Manager.AM.Stop3DSFX(clipToStop);
            else Audio_Manager.AM.Stop2DSFX(clipToStop);
        }

        public override string ToString()
        {
            return "Stop Looping Sound Effect";
        }
    }
}
