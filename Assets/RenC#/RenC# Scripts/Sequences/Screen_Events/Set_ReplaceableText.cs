using UnityEngine;
namespace RenCSharp.Sequences
{
    public class Set_ReplaceableText : Screen_Event
    {
        [SerializeField, Tooltip("Text pattern that will be replaced.")] private string textToReplace;
        [SerializeField, Tooltip("What the text pattern will be replaced with.")] private string replacingText;

        public override void DoEvent()
        {
            Textbox_String.AddReplacableText(textToReplace, replacingText);
        }

        public override string ToString()
        {
            return "Set Replaceable Text";
        }
    }
}
