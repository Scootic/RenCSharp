using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using RenCSharp.Tags;
using EXPERIMENTAL;
namespace RenCSharp
{
    public static class Textbox_String
    {
        private static Dictionary<string, string> replacerTexts = new();
        /// <summary>
        /// Since there's only one textbox open at a time, I hope, doing things with static parameters SHOULD work.
        /// </summary>
        public static float TextSpeed = 0.1f;
        /// <summary>
        /// Makes the coroutine skip past the filling in char by char and just displays final text
        /// </summary>
        public static bool JumpToEndOfTextbox = false;
        private static bool pausedTextbox = false;
        /// <summary>
        /// Does animated text for a textbox, one character at a time. can be forced to fill in every other character by setting
        /// JumpToEndOfTextbox to true. Idles on a character if pausedTextbox is true.
        /// </summary>
        /// <param name="textBox">The text area being populated by the text.</param>
        /// <param name="endText">What the text should look like when done.</param>
        /// <returns>Peace in our time.</returns>
        public static IEnumerator RunThroughText(TextMeshProUGUI textBox, string endText)
        {
            float t = 0;
            int i = 0;
            string amended = endText;
            TagParser.SetCurrentTextMesh = textBox;

            amended = ReplaceableText(amended);
            amended = TagParser.CleanOutFlags(amended);

            string tagless = TagParser.CleanOutTags(amended, false);
            char[] dialogchars = amended.ToCharArray();
            textBox.text = ""; //empty box before repopulating below
            JumpToEndOfTextbox = false;

            while (dialogchars.Length > textBox.text.Length && tagless.Length > textBox.text.Length && !JumpToEndOfTextbox)
            {
                //only run through text if the SM is unpaused
                while (pausedTextbox)
                {
                    yield return null;
                }

                t += Time.deltaTime;
                //add one character at a time, depending on text speed
                if (t >= TextSpeed && i < dialogchars.Length)
                {
                    t = 0;

                    if (dialogchars[i] == '<') //we've found a rich text tag
                    {
                        string tag = "" + dialogchars[i]; //collect all the chars that make up our tag
                        while (dialogchars[i] != '>')
                        {
                            i++;
                            tag += dialogchars[i];
                        }
                        i++;

                        if (!TagParser.Parse(tag)) //if it's not a tagparser tag, it's probably unity valid. add that boy back in.
                        {
                            textBox.text += tag;
                        }
                        else //remove tags from the final display if it's being handled by tag parser
                        {
                            amended = Regex.Replace(amended, tag, "");
                        }
                    }
                    else //just add the char and move on if it's a regular ah character
                    {
                        textBox.text += dialogchars[i];
                        textBox.ForceMeshUpdate();
                        TMP_CharacterInfo c = textBox.textInfo.characterInfo[textBox.text.Length - 1];
                        Event_Bus.TryFireSingleObjEvent("TextboxNewChar", (object)c);
                        i++;
                    }
                }

                yield return null;
            }

            JumpToEndOfTextbox = true;
            textBox.text = TagParser.CleanOutTags(amended);
        }
        /// <summary>
        /// Goes through a string, and replaces all instances of keys in the replaceabletexts dictionary with their values.
        /// Ie. Replace {mc} with the name stored in savedata, etc.
        /// </summary>
        /// <param name="sInput">The text you want parsed.</param>
        /// <returns>The input with all replaceable texts replaced.</returns>
        public static string ReplaceableText(string sInput)
        {
            string sOutput = sInput;
            foreach (KeyValuePair<string, string> kvp in replacerTexts) //by the end of this, replace generic guys like {mc} with the actual player's name
            {
                //Debug.Log("Doing a stupid replacering!");
                sOutput = Regex.Replace(sOutput, kvp.Key, kvp.Value);
            }
            return sOutput;
        }
        /// <summary>
        /// Stops any textbox from displaying new chars, hover on current string instead.
        /// </summary>
        /// <param name="stop">What we set the paused value to. True to stop, False to go.</param>
        public static void PauseTextbox(bool stop)
        {
            pausedTextbox = stop;
        }

        /// <summary>
        /// Strings fed into the RunThroughText IEnumerator are parsed by a dictionary of strings. It will replace any instance
        /// of the pattern that's the key, with the pattern that's the value. Regex moment. This adds a KVP to that dictionary.
        /// </summary>
        /// <param name="replaced">The string pattern that will be replaced.</param>
        /// <param name="replacer">What the replaced string pattern will be replaced by.</param>
        public static void AddReplacableText(string replaced, string replacer)
        {
            if (!replacerTexts.ContainsKey(replaced))
            {
                replacerTexts.Add(replaced, replacer);
            }
            else
            {
                Debug.LogWarning("The replacer texts dictionary already contains: " + replaced + ". Setting value to: " + replacer);
                replacerTexts[replaced] = replacer;
            }
        }
    }
}
