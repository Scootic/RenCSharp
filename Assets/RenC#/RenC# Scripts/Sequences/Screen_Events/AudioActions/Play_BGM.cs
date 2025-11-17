using UnityEngine;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Plays a new music track and fades out the old one.
    /// </summary>
    public class Play_BGM : Screen_Event
    {
        [SerializeField] private AudioClip musicTrack;
        [SerializeField,Min(0), Tooltip("0 for no fade out.")] private float fadeTime = 1f;
        [SerializeField, Tooltip("Sets the new audio track to start at current track's duration.")] private bool setToSameTime = false;
        public override void DoShit()
        {
            Audio_Manager.AM.PlayBGM(musicTrack, fadeTime, true, setToSameTime);
        }

        public override string ToString()
        {
            return "Play Music Track";
        }
    }
}
