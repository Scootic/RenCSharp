using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Play_SFX : Screen_Event
    {
        [SerializeField] private AudioClip sfxToPlay;
        [SerializeField, Tooltip("Leave as Vec3.zero to be a 2D sfx")] private Vector3 position = Vector3.zero;
        [SerializeField] private bool environmental = false;
        public override void DoShit()
        {
            if (position == Vector3.zero) Audio_Manager.AM.Play2DSFX(sfxToPlay);
            else Audio_Manager.AM.Play3DSFX(sfxToPlay, position, environmental);
        }

        public override string ToString()
        {
            return "Play Sound Effect";
        }
    }
}
