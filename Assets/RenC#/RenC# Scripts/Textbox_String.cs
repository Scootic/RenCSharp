using System.Collections;
using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using RenCSharp.Tags;
using RenCSharp.EXPERIMENTAL;
namespace RenCSharp
{
    /// <summary>
    /// This is a static class to handle textboxes in a better way. Anyone can reference this, but you'll need some TMP juices.
    /// </summary>
    public static class Textbox_String
    {
        public static Dictionary<string, string> GetReplacerTexts => replacerTexts;
        /// <summary>
        /// Stupid dumb dictionary that contains key strings that we want replaced by value strings. Ie. replacing any instance of {mc} with the
        /// inputted name given by player. Used by the RunThroughText coroutine
        /// </summary>
        private static Dictionary<string, string> replacerTexts = new();
        /// <summary>
        /// Since there's only one textbox open at a time, I hope, doing things with static parameters SHOULD work. The seconds it takes for next 
        /// char to be displayed by RunThroughText().
        /// </summary>
        public static float TextSpeed = 0.1f;
        /// <summary>
        /// Makes the coroutine skip past the filling in char by char and just displays final text
        /// </summary>
        public static bool JumpToEndOfTextbox = false;
        /// <summary>
        /// Makes sure the coroutine will idle if the player is paused or something else is going on, like a fade transition.
        /// </summary>
        private static bool pausedTextbox = false;
        /// <summary>
        /// Does animated text for a textbox, one character at a time. can be forced to fill in every other character by setting
        /// JumpToEndOfTextbox to true. Idles on a character if pausedTextbox is true.
        /// </summary>
        /// <param name="textBox">The text area being populated by the text.</param>
        /// <param name="endText">The stupid dumb string that the textbox will be filled with</param>
        /// <returns>Peace in our time.</returns>
        public static IEnumerator RunThroughText(TextMeshProUGUI textBox, string endText)
        {
            float t = 0;
            int i = 0;
            string amended = endText;
            TagParser.SetCurrentTextMesh = textBox;

            amended = ReplaceableText(amended);
            amended = TagParser.CleanOutFlags(amended);

            textBox.text = amended; //insert text that will be shown over time

            textBox.maxVisibleCharacters = 0;
            textBox.ForceMeshUpdate();
            JumpToEndOfTextbox = false;

            while (i < textBox.text.Length && !JumpToEndOfTextbox) //while running through a textbox (player hasn't done nothing)
            {
                //only run through text if the SM is unpaused
                while (pausedTextbox)
                {
                    yield return null;
                }

                t += Time.deltaTime;
                //add one character at a time, depending on text speed
                if (t >= TextSpeed)
                {
                    t = 0;
                    if (textBox.text[i] == '<') //we've found a rich text tag
                    {
                        string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                        while (textBox.text[i] != '>')
                        {
                            i++;
                            tag += textBox.text[i];
                        }

                        if (TagParser.Parse(tag))
                        {
                            string s = Regex.Replace(textBox.text, tag, "");
                            textBox.text = s;
                            textBox.ForceMeshUpdate();
                            i -= tag.Length - 1; //??
                        }
                    }
                    else //just add the char and move on if it's a regular ah character
                    {
                        textBox.maxVisibleCharacters++;
                        textBox.ForceMeshUpdate();
                        //only care about passing in a stupid char for the event if event actually exists
                        if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                        {
                            int goodI = TagParser.StringIndexExcludeBuiltinTags(textBox.text, i, true); //since the text string gets rid of valid tags???
                            TMP_CharacterInfo c = textBox.textInfo.characterInfo[goodI];
                            Event_Bus.TryFireSingleObjEvent("TextboxNewChar", (object)c);
                        }
                        i++;
                    }
                }

                yield return null;
            }

            JumpToEndOfTextbox = true;

            while(i < textBox.text.Length) //just run through everything in a single frame if player skipped ahead.
            {
                if (textBox.text[i] == '<') //we've found a rich text tag
                {
                    string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                    while (textBox.text[i] != '>')
                    {
                        i++;
                        tag += textBox.text[i];
                    }

                    if (TagParser.Parse(tag))
                    {
                        string s = Regex.Replace(textBox.text, tag, "");
                        textBox.text = s;
                        i -= tag.Length - 1; //??
                    }
                }
                else //just add the char and move on if it's a regular ah character
                {
                    textBox.maxVisibleCharacters++;
                    //only care about passing in a stupid char for the event if event actually exists
                    if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                    {
                        int goodI = TagParser.StringIndexExcludeBuiltinTags(textBox.text, i);
                        TMP_CharacterInfo c = textBox.textInfo.characterInfo[goodI];
                        stu.Invoke((object)c);
                    }
                    i++;
                }
            }
            textBox.ForceMeshUpdate();
            textBox.maxVisibleCharacters = textBox.text.Length; //please show everything if we aren't already!
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

        public static void ReceiveReplacableTextFromSave(string[] replaced, string[] replacers)
        {
            replacerTexts = new();
            //replaced[] and replacers[] SHOULD be the same length, since they're saved into file by grabbing from the dict
            for (int i = 0; i < replaced.Length; i++)
            {
                replacerTexts.Add(replaced[i], replacers[i]);
            }
        }
    }
}
