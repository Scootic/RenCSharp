using System;
using UnityEngine;

namespace RenCSharp.Sequences
{
    /// <summary>
    /// Stops a sound effect. Should only be used with SFX that you leave looping indefinetly to prevent chicaney.
    /// </summary>
    [Obsolete("Outdated, use Stop_SFXAsset to communicate with Save/Load.", false)]
    public class Stop_SFX : Screen_Event
    {
        [SerializeField] private AudioClip clipToStop;
        [SerializeField] protected bool is3D = false;

        public AudioClip GetClipToStop;
        public bool GetIs3D => is3D;

        public override void DoEvent()
        {
            if (is3D) Audio_Manager.AM.Stop3DSFX(clipToStop);
            else Audio_Manager.AM.Stop2DSFX(clipToStop);
        }

        public override string ToString()
        {
            return "Deprecated/Stop Looping Sound Effect";
        }
    }
}
