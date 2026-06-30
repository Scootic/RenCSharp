using TMPro;
using UnityEngine;
namespace RenCSharp.Sequences
{
    public class Set_ReplaceableText_Prompt : Screen_Event
    {
        [SerializeField, Tooltip("Basically the label of the input prompt.")] private string queryText;
        [SerializeField, Tooltip("The string pattern that will be replaced whenever dialog is rendered. (Like how player name replaces {mc})")] private string textToReplace;
        [SerializeField, Tooltip("Prefab that spawns to take in string value. Requires an inputfield and button.")] private GameObject replacerPrefab;

        private UI_Element uie;
        private TMP_InputField inputter;
        private string replacingText;
        public override void DoEvent()
        {
            Script_Manager.SM.PauseSequence(false);
            uie = Object_Factory.SpawnObject(replacerPrefab, "Replacer Field", Script_Manager.SM.PlayerChoiceHolder).GetComponent<UI_Element>();
            uie.Buttons[0].interactable = false;
            uie.Texts[0].text = queryText;
            inputter = uie.transform.GetChild(0).GetComponent<TMP_InputField>();
            inputter.onEndEdit.AddListener(delegate
            {
                uie.Buttons[0].interactable = inputter.text != "";
                replacingText = inputter.text;
            });

            uie.Buttons[0].onClick.AddListener(SetTheString);
        }

        void SetTheString()
        {
            Textbox_String.AddReplacableText(textToReplace, replacingText);
            Script_Manager.SM.UnpauseSequence();
            Object_Factory.RemoveObject("Replacer Field");
        }

        public override string ToString()
        {
            return "Set Replaceable Text Prompt";
        }
    }
}
