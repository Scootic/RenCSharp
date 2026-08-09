using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RenCSharp.Sequences
{
    /// <summary>
    /// Stops a sound effect. Should only be used with SFX that you leave looping indefinetly to prevent chicaney.
    /// </summary>
    [Obsolete("Outdated, use Stop_SFXAsset to communicate with Save/Load.", false)]
    public class Stop_SFX : Screen_Event, IDeprecatedReplaceable<Screen_Event>
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

        public Screen_Event Replacement()
        {
            Stop_SFXAsset newVentSS = new Stop_SFXAsset();

            newVentSS.SetIs3D = is3D;
            string johnsonSS = AssetDatabase.GetAssetPath(clipToStop);
            string guidSS = AssetDatabase.AssetPathToGUID(johnsonSS);
            newVentSS.SetSFXToStop = new AssetReference(guidSS);

            return newVentSS;
        }


        public override string ToString()
        {
            return "Deprecated/Stop Looping Sound Effect";
        }
    }
}
