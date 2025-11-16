using UnityEngine;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// The primary assets that will make up a visual novel. They contain screens, which are the serious of dialog boxes and events that happen
    /// in the game, and player choices. Player choices are simple, only having a string that will appear on the button that spawns, and a sequence that
    /// gets loaded when you pick it.
    /// 
    /// If it helps, you can think of them as nodes on a tree that are connected to one another by player choices. (Although there is currently no
    /// way to visualize that.)
    /// </summary>
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
