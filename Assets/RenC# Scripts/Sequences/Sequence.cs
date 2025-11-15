using UnityEngine;
namespace RenCSharp.Sequences
{
    [CreateAssetMenu(menuName = "New Sequence")]
    public class Sequence : ScriptableObject
    {
        [SerializeField] private bool autoFocusSpeaker;

        [SerializeField] private Screen[] screens;

        [SerializeField] private Player_Choice[] playerChoices;

        public bool AutoFocusSpeaker => autoFocusSpeaker;
        public Screen[] Screens => screens;
        public Player_Choice[] PlayerChoices => playerChoices;
    }
}
