using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
namespace RenCSharp
{
    public static class Textbox_String
    {
        /// <summary>
        /// Since there's only one textbox open at a time, I hope, doing things with static parameters SHOULD work.
        /// </summary>
        public static float TextSpeed = 0.1f;
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
            char[] dialogchars = endText.ToCharArray();
            textBox.text = "";

            while (dialogchars.Length > textBox.text.Length && endText.Length > textBox.text.Length && !JumpToEndOfTextbox)
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
                        string tag = "" + dialogchars[i];
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
                            endText = Regex.Replace(endText, tag, "");
                        }
                    }
                    else //just add the char and move on if it's a regular ah character
                    {
                        textBox.text += dialogchars[i];
                        i++;
                    }
                }

                yield return null;
            }

            JumpToEndOfTextbox = true;
            textBox.text = TagParser.CleanOutTags(endText);
        }

        public static void PauseTextbox(bool t)
        {
            pausedTextbox = t;
        }
    }
}
