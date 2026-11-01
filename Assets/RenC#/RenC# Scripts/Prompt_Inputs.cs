using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RenCSharp.EXPERIMENTAL;
namespace RenCSharp
{
    /// <summary>
    /// Class for giving players an input prompt that can be awaited, pausing behavior until the player makes a selection or input.
    /// </summary>
    public static class Prompt_Inputs
    {
        private static bool receivedInput;
        /// <summary>
        /// Spawns a TextInputPrompt that hovers until the player submits text. This submitted text invokes the given action.
        /// </summary>
        /// <param name="promptUIEPrefab">Prefab of input field, requires a UIE with a button and text, and a child TMP_InputField.</param>
        /// <param name="parent">Transform parent that the prefab spawns in.</param>
        /// <param name="queryText">Will have flags and replaceable text replaced.</param>
        /// <param name="result">The action that will be invoked with the InputField's text.</param>
        /// <returns></returns>
        public static async Awaitable TextInputPrompt(GameObject promptUIEPrefab, Transform parent, string queryText, Action<string> result)
        {
            receivedInput = false;

            if(!Object_Factory.SpawnObject(promptUIEPrefab, "textinput", parent).TryGetComponent(out UI_Element uie))
            {
                Debug.LogError("TextInputPrompt prefab doesn't have a UI_Element component.");
                return;
            }
            if(!uie.transform.GetChild(0).TryGetComponent(out TMP_InputField inputField))
            {
                Debug.LogError("TextInputPrompt prefab doesn't have a TMP_InputField component on its first child object.");
                return;
            }

            uie.Buttons[0].interactable = false;
            uie.Texts[0].text = Flag_Manager.CleanOutFlags(Textbox_String.ReplaceableText(queryText));
            inputField.onEndEdit.AddListener((s) =>
            {
                uie.Buttons[0].interactable = s != "";
            });
            uie.Buttons[0].onClick.AddListener(() =>
            {
                receivedInput = true;
            });

            while (!receivedInput)
            {
                await Awaitable.NextFrameAsync();
            }
            result?.Invoke(inputField.text);
            Object_Factory.RemoveObject("textinput");
        }
        /// <summary>
        /// Spawns a list of buttons for the player to choose between, which fires the corresponding action when chosen.
        /// </summary>
        /// <param name="buttonPrefab">The prefab of the button. Requires a button component, optional TextScaler component.</param>
        /// <param name="parent">Transform parent that the prefab will spawn in. Assumed to have some sort of layout group to automatically handle button positions.</param>
        /// <param name="buttonLabels">The string text displayed on the buttons.</param>
        /// <param name="buttonResults">The possible results of pressing one button instead of another.</param>
        /// <returns></returns>
        public static async Awaitable ListButtonInputPrompt(GameObject buttonPrefab, Transform parent, string[] buttonLabels, Action[] buttonResults)
        {
            receivedInput = false;

            for (int i = 0; i < buttonLabels.Length; i++)
            {
                buttonLabels[i] = Flag_Manager.CleanOutFlags(Textbox_String.ReplaceableText(buttonLabels[i]));
            }

            GameObject[] activeButtons = new GameObject[buttonLabels.Length];
            Button b;

            for(int i = 0; i < activeButtons.Length; i++)
            {
                int oldI = i;
                activeButtons[oldI] = Object_Factory.SpawnObject(buttonPrefab, $"buttoninput{oldI}", parent);
                if (!activeButtons[oldI].TryGetComponent(out b))
                {
                    Debug.LogError("ListButtonInputPrompt prefab doesn't have a button component.");
                    return;
                }
                b.GetComponentInChildren<TextMeshProUGUI>().text = buttonLabels[oldI];
                if(b.TryGetComponent(out TextHolder_Scaler ths))
                {
                    ths.ScaleTextHolder();
                }
                b.onClick.AddListener(() =>
                {
                    receivedInput = true;
                    buttonResults[oldI]?.Invoke();
                });
            }

            while (!receivedInput)
            {
                await Awaitable.NextFrameAsync();
            }

            for(int i = activeButtons.Length - 1; i >= 0; i--)
            {
                Object_Factory.RemoveObject($"buttoninput{i}");
            }
        }
    }
}
