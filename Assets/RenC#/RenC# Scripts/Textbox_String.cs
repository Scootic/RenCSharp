using RenCSharp.EXPERIMENTAL;
using RenCSharp.Tags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UObject = UnityEngine.Object;
namespace RenCSharp
{
    /// <summary>
    /// This is a static class to handle textboxes in a better way. Anyone can reference this, but you'll need some TMP juices.
    /// </summary>
    public static class Textbox_String
    {
        private const string WSPBreak = "\u200B";
        public static Dictionary<string, string> GetReplacerTexts => replacerTexts;
        /// <summary>
        /// Stupid dumb dictionary that contains key strings that we want replaced by value strings. Ie. replacing any instance of {mc} with the
        /// inputted name given by player. Used by the RunThroughText coroutine. Certified Regex moment.
        /// </summary>
        private static Dictionary<string, string> replacerTexts = new();
        /// <summary>
        /// Since there's only one textbox open at a time, I hope, doing things with static parameters SHOULD work. The seconds it takes for next 
        /// char to be displayed by RunThroughText().
        /// </summary>
        public static float TextSpeed = 0.1f;
        /// <summary>
        /// For character fading in animation.
        /// </summary>
        private static float FadeCharSpeed => Mathf.Max(0.2f, TextSpeed);
        /// <summary>
        /// Makes the coroutine skip past the filling in char by char and just displays final text
        /// </summary>
        public static bool JumpToEndOfTextbox = false;
        /// <summary>
        /// Makes sure the coroutine will idle if the player is paused or something else is going on, like a fade transition.
        /// </summary>
        private static bool pausedTextbox = false;
        /// <summary>
        /// Regex used when excluding custom rich-tags from being rendered in-game. Declared in class to do less GC.
        /// </summary>
        private static Regex excludeTagEx;
        /// <summary>
        /// Determines the curve by which characters fade in over time when shown.
        /// </summary>
        private static AnimationCurve characterFadeInCurve = Animation_Helper.EaseOut;
        /// <summary>
        /// Sets the curve that determines how characters fade in over time when shown by FadeInALetterAsync()
        /// </summary>
        public static AnimationCurve SetCharacterFadeInCurve { set { characterFadeInCurve = value; } }
        /// <summary>
        /// Does animated text for a textbox, one character at a time. can be forced to fill in every other character by setting
        /// JumpToEndOfTextbox to true. Idles on a character if pausedTextbox is true.
        /// </summary>
        /// <param name="textBox">The text area being populated by the text.</param>
        /// <param name="endText">The stupid dumb string that the textbox will be filled with</param>
        /// <returns>Peace in our time.</returns>
        public static IEnumerator RunThroughText(TMP_Text textBox, string endText, bool debugTextbox = false)
        {
            textBox.text = "";
            textBox.ForceMeshUpdate();
            while (pausedTextbox) yield return null; //No friggin' idea why this is necessary for first runtime screen?????????

            float t = 0;
            int i = 0; //"real" string char index
            int charI = 0; //tmp_charInfo index (excludes built-in rich tags already... nightmare nightmare parsing!)
            string amended = endText;
            TMP_CharacterInfo curChar;

            TagParser.SetCurrentTextMesh = textBox;

            amended = ReplaceableText(amended);
            amended = Flag_Manager.CleanOutFlags(amended);

            textBox.text = amended; //set the text that will be show over time/mangled horrifically by tags
            textBox.fontSize = PredictFontSize(TagParser.CleanOutTags(amended, false), textBox);
            textBox.ForceMeshUpdate();
            HideLetters(textBox);

            JumpToEndOfTextbox = false;

            string s = PlaceInvisibleCharsBetweenTags(amended, debugTextbox);

            while (charI < textBox.textInfo.characterCount && !JumpToEndOfTextbox && i < textBox.text.Length) //while running through a textbox (player hasn't done nothing)
            {
                //only run through text if unpaused
                while (pausedTextbox) yield return null;

                t += Time.deltaTime;
                //add one character at a time, depending on text speed
                if (t >= TextSpeed)
                {
                    t = 0;
                    if (textBox.text[i] == '<') //we've found a rich text tag
                    {
                        //Debug.Log("Touched rich tag?");
                        string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                        while (textBox.text[i] != '>')
                        {
                            i++;
                            if(i >= textBox.text.Length)
                            {
                                Debug.LogError("Tag found that has no closer. Stopping RunThroughText!");
                                yield break;
                            }
                            tag += textBox.text[i];
                        }
                        i++;
                        if (TagParser.Parse(tag))
                        {
                            ///Debug.Log("Touched rich tag!");
                            //Because {s} has invisible chars before and after tags for line-spacing concerns, we have to
                            //take that into account when Regex-ing them away.
                            excludeTagEx = new Regex(WSPBreak + tag + WSPBreak);
                            s = excludeTagEx.Replace(s, "", 1);
                            ///if(debugTextbox)Debug.Log($"PredictVerts string: {s}, stringL: {s.Length}");
                            ExcludeLetters(textBox, charI, charI + tag.Length, s, debugTextbox);
                            charI += tag.Length;//?
                        }
                    }
                    if (i >= textBox.text.Length) break;
                    curChar = textBox.textInfo.characterInfo[charI];
                    if (curChar.character != ' ' && curChar.character != '<')
                    {
                        if (PlayerPrefs.GetInt("AnimateChars", 1) == 1) _ = FadeInALetterAsync(textBox, charI);
                        else ShowALetter(textBox, charI, debugTextbox);
                        //only care about passing in a stupid char for the event if event actually exists
                        if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                        {
                            try
                            {
                                //pass in the current character if applicable.
                                stu?.Invoke((object)textBox.textInfo.characterInfo[charI]);
                            }
                            catch
                            {
                                Debug.LogWarning("For some stupid reason, Textbox_String is trying to fire the spawn new character event on an invalid indexed character?!? Wild.");
                            }
                        }
                    }
                    charI++;
                    i++;
                }

                yield return null;
            }

            JumpToEndOfTextbox = true;

            while (charI < textBox.textInfo.characterCount && i < textBox.text.Length) //just run through everything in a single frame if player skipped ahead.
            {
                if (textBox.text[i] == '<') //we've found a rich text tag
                {
                    string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                    while (textBox.text[i] != '>')
                    {
                        i++;
                        tag += textBox.text[i];
                    }
                    i++;
                    //don't need to bother excluding things if the last letter of tag is the last letter of the string?
                    //might leave a gap big enough to cause smaller text size, but might be preferrable to FURTHER index chicanery
                    if (TagParser.Parse(tag))
                    {
                        excludeTagEx = new Regex(tag);
                        s = excludeTagEx.Replace(s, "", 1);
                        ExcludeLetters(textBox, charI, charI + tag.Length, s, debugTextbox);
                        charI += tag.Length; //
                    }
                }
                if (i >= textBox.text.Length) break;
                curChar = textBox.textInfo.characterInfo[charI];
                if (curChar.character != ' ' && curChar.character != '<')
                {
                    if (PlayerPrefs.GetInt("AnimateChars", 1) == 1) _ = FadeInALetterAsync(textBox, charI);
                    else ShowALetter(textBox, charI, debugTextbox);
                    //only care about passing in a stupid char for the event if event actually exists
                    if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                    {
                        try
                        {
                            //pass in the current character if applicable.
                            stu?.Invoke((object)textBox.textInfo.characterInfo[charI]);
                        }
                        catch
                        {
                            Debug.LogWarning("For some stupid reason, Textbox_String is trying to fire the spawn new character event on an invalid indexed character?!? Wild.");
                        }
                    }
                }
                charI++;
                i++;
            }
        }
        /// <summary>
        /// Does animated text for a textbox, one character at a time. can be forced to fill in every other character by setting
        /// JumpToEndOfTextbox to true. Idles on a character if pausedTextbox is true.
        /// </summary>
        /// <param name="textBox">The text area being populated by the text.</param>
        /// <param name="endText">The stupid dumb string that the textbox will be filled with</param>
        /// <returns>Peace in our time.</returns>
        public static IEnumerator RunThroughText(TMP_Text textBox, string endText, AudioClip dialogSound, float basePitch, float pitchVariance = 0.1f, bool debugTextbox = false)
        {
            textBox.text = "";
            textBox.ForceMeshUpdate();
            while (pausedTextbox) yield return null; //No friggin' idea why this is necessary for first runtime screen?????????

            float t = 0;
            int i = 0; //"real" string char index
            int charI = 0; //tmp_charInfo index (excludes built-in rich tags already... nightmare nightmare parsing!)
            string amended = endText;
            TMP_CharacterInfo curChar;

            TagParser.SetCurrentTextMesh = textBox;

            amended = ReplaceableText(amended);
            amended = Flag_Manager.CleanOutFlags(amended);

            textBox.text = amended; //set the text that will be show over time/mangled horrifically by tags
            textBox.fontSize = PredictFontSize(TagParser.CleanOutTags(amended, false), textBox);
            textBox.ForceMeshUpdate();
            HideLetters(textBox);

            JumpToEndOfTextbox = false;

            string s = PlaceInvisibleCharsBetweenTags(amended, debugTextbox);

            while (charI < textBox.textInfo.characterCount && !JumpToEndOfTextbox && i < textBox.text.Length) //while running through a textbox (player hasn't done nothing)
            {
                //only run through text if unpaused
                while (pausedTextbox) yield return null;

                t += Time.deltaTime;
                //add one character at a time, depending on text speed
                if (t >= TextSpeed)
                {
                    
                    t = 0;
                    if (textBox.text[i] == '<') //we've found a rich text tag
                    {
                        //Debug.Log("Touched rich tag?");
                        string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                        while (textBox.text[i] != '>')
                        {
                            i++;
                            if (i >= textBox.text.Length)
                            {
                                Debug.LogError("Tag found that has no closer. Stopping RunThroughText!");
                                yield break;
                            }
                            tag += textBox.text[i];
                        }
                        i++;
                        if (TagParser.Parse(tag))
                        {
                            ///Debug.Log("Touched rich tag!");
                            //Because {s} has invisible chars before and after tags for line-spacing concerns, we have to
                            //take that into account when Regex-ing them away.
                            excludeTagEx = new Regex(WSPBreak + tag + WSPBreak);
                            s = excludeTagEx.Replace(s, "", 1);
                            ///if(debugTextbox)Debug.Log($"PredictVerts string: {s}, stringL: {s.Length}");
                            ExcludeLetters(textBox, charI, charI + tag.Length, s, debugTextbox);
                            charI += tag.Length;//?
                        }
                    }
                    if (i >= textBox.text.Length) break;
                    curChar = textBox.textInfo.characterInfo[charI];
                    if (curChar.character != ' ' && curChar.character != '<')
                    {
                        Audio_Manager.AM.Play2DSFX(dialogSound, basePitch - pitchVariance, basePitch + pitchVariance, 1, true, true);
                        if (PlayerPrefs.GetInt("AnimateChars", 1) == 1) _ = FadeInALetterAsync(textBox, charI);
                        else ShowALetter(textBox, charI, debugTextbox);
                        //only care about passing in a stupid char for the event if event actually exists
                        if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                        {
                            try
                            {
                                //pass in the current character if applicable.
                                stu?.Invoke((object)textBox.textInfo.characterInfo[charI]);
                            }
                            catch
                            {
                                Debug.LogWarning("For some stupid reason, Textbox_String is trying to fire the spawn new character event on an invalid indexed character?!? Wild.");
                            }
                        }
                    }
                    charI++;
                    i++;
                }

                yield return null;
            }

            JumpToEndOfTextbox = true;

            while (charI < textBox.textInfo.characterCount && i < textBox.text.Length) //just run through everything in a single frame if player skipped ahead.
            {
                if (textBox.text[i] == '<') //we've found a rich text tag
                {
                    string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                    while (textBox.text[i] != '>')
                    {
                        i++;
                        tag += textBox.text[i];
                    }
                    i++;
                    //don't need to bother excluding things if the last letter of tag is the last letter of the string?
                    //might leave a gap big enough to cause smaller text size, but might be preferrable to FURTHER index chicanery
                    if (TagParser.Parse(tag))
                    {
                        excludeTagEx = new Regex(tag);
                        s = excludeTagEx.Replace(s, "", 1);
                        ExcludeLetters(textBox, charI, charI + tag.Length, s, debugTextbox);
                        charI += tag.Length; //
                    }
                }
                if (i >= textBox.text.Length) break;
                curChar = textBox.textInfo.characterInfo[charI];
                if (curChar.character != ' ' && curChar.character != '<')
                {
                    if (PlayerPrefs.GetInt("AnimateChars", 1) == 1) _ = FadeInALetterAsync(textBox, charI);
                    else ShowALetter(textBox, charI, debugTextbox);
                    //only care about passing in a stupid char for the event if event actually exists
                    if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                    {
                        try
                        {
                            //pass in the current character if applicable.
                            stu?.Invoke((object)textBox.textInfo.characterInfo[charI]);
                        }
                        catch
                        {
                            Debug.LogWarning("For some stupid reason, Textbox_String is trying to fire the spawn new character event on an invalid indexed character?!? Wild.");
                        }
                    }
                }
                charI++;
                i++;
            }
        }
        /// <summary>
        /// Async version of the RunThroughText coroutine. Basically all of the same logic, but instead of being
        /// handled as a coroutine, it relies on the Awaitable functions (ie. 'await Awaitable.NextFrameAsync' instead of 'yield
        /// return null').
        /// </summary>
        /// <param name="textBox">The text area being populated by text.</param>
        /// <param name="endText">How the final text display will look (tags not withstanding).</param>
        /// <returns>War in our time?!?</returns>
        public static async Awaitable RunThroughTextAsync(TMP_Text textBox, string endText, bool debugTextbox = false)
        {
            textBox.text = "";
            textBox.ForceMeshUpdate();
            while (pausedTextbox) await Awaitable.NextFrameAsync(); //No friggin' idea why this is necessary for first runtime screen?????????

            float t = 0;
            int i = 0; //"real" string char index
            int charI = 0; //tmp_charInfo index (excludes built-in rich tags already... nightmare nightmare parsing!)
            string amended = endText;
            TMP_CharacterInfo curChar;

            TagParser.SetCurrentTextMesh = textBox;

            amended = ReplaceableText(amended);
            amended = Flag_Manager.CleanOutFlags(amended);

            textBox.text = amended; //set the text that will be show over time/mangled horrifically by tags
            textBox.fontSize = PredictFontSize(TagParser.CleanOutTags(amended, false), textBox);
            textBox.ForceMeshUpdate();
            HideLetters(textBox);

            JumpToEndOfTextbox = false;

            string s = PlaceInvisibleCharsBetweenTags(amended, debugTextbox);

            while (charI < textBox.textInfo.characterCount && !JumpToEndOfTextbox && i < textBox.text.Length) //while running through a textbox (player hasn't done nothing)
            {
                //only run through text if unpaused
                while (pausedTextbox) await Awaitable.NextFrameAsync();

                t += Time.deltaTime;
                //add one character at a time, depending on text speed
                if (t >= TextSpeed)
                {
                    t = 0;
                    if (textBox.text[i] == '<') //we've found a rich text tag
                    {
                        //Debug.Log("Touched rich tag?");
                        string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                        while (textBox.text[i] != '>')
                        {
                            i++;
                            if (i >= textBox.text.Length)
                            {
                                Debug.LogError("Tag found that has no closer. Stopping RunThroughTextAsync!");
                                return;
                            }
                            tag += textBox.text[i];
                        }
                        i++;
                        if (TagParser.Parse(tag))
                        {
                            ///Debug.Log("Touched rich tag!");
                            //Because {s} has invisible chars before and after tags for line-spacing concerns, we have to
                            //take that into account when Regex-ing them away.
                            excludeTagEx = new Regex(WSPBreak + tag + WSPBreak);
                            s = excludeTagEx.Replace(s, "", 1);
                            ///if(debugTextbox)Debug.Log($"PredictVerts string: {s}, stringL: {s.Length}");
                            ExcludeLetters(textBox, charI, charI + tag.Length, s, debugTextbox);
                            charI += tag.Length;//?
                        }
                    }
                    if (i >= textBox.text.Length) break;
                    curChar = textBox.textInfo.characterInfo[charI];
                    if (curChar.character != ' ' && curChar.character != '<')
                    {
                        if (PlayerPrefs.GetInt("AnimateChars", 1) == 1) _ = FadeInALetterAsync(textBox, charI);
                        else ShowALetter(textBox, charI, debugTextbox);
                        //only care about passing in a stupid char for the event if event actually exists
                        if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                        {
                            try
                            {
                                //pass in the current character if applicable.
                                stu?.Invoke((object)textBox.textInfo.characterInfo[charI]);
                            }
                            catch
                            {
                                Debug.LogWarning("For some stupid reason, Textbox_String is trying to fire the spawn new character event on an invalid indexed character?!? Wild.");
                            }
                        }
                    }
                    charI++;
                    i++;
                }

                await Awaitable.NextFrameAsync();
            }

            JumpToEndOfTextbox = true;

            while (charI < textBox.textInfo.characterCount && i < textBox.text.Length) //just run through everything in a single frame if player skipped ahead.
            {
                if (textBox.text[i] == '<') //we've found a rich text tag
                {
                    string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                    while (textBox.text[i] != '>')
                    {
                        i++;
                        tag += textBox.text[i];
                    }
                    i++;
                    //don't need to bother excluding things if the last letter of tag is the last letter of the string?
                    //might leave a gap big enough to cause smaller text size, but might be preferrable to FURTHER index chicanery
                    if (TagParser.Parse(tag))
                    {
                        excludeTagEx = new Regex(tag);
                        s = excludeTagEx.Replace(s, "", 1);
                        ExcludeLetters(textBox, charI, charI + tag.Length, s, debugTextbox);
                        charI += tag.Length; //
                    }
                }
                if (i >= textBox.text.Length) break;
                curChar = textBox.textInfo.characterInfo[charI];
                if (curChar.character != ' ' && curChar.character != '<')
                {
                    if (PlayerPrefs.GetInt("AnimateChars", 1) == 1) _ = FadeInALetterAsync(textBox, charI);
                    else ShowALetter(textBox, charI, debugTextbox);
                    //only care about passing in a stupid char for the event if event actually exists
                    if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                    {
                        try
                        {
                            //pass in the current character if applicable.
                            stu?.Invoke((object)textBox.textInfo.characterInfo[charI]);
                        }
                        catch
                        {
                            Debug.LogWarning("For some stupid reason, Textbox_String is trying to fire the spawn new character event on an invalid indexed character?!? Wild.");
                        }
                    }
                }
                charI++;
                i++;
            }
        }
        /// <summary>
        /// Async version of the RunThroughText coroutine. Basically all of the same logic, but instead of being
        /// handled as a coroutine, it relies on the Awaitable functions (ie. 'await Awaitable.NextFrameAsync' instead of 'yield
        /// return null').
        /// </summary>
        /// <param name="textBox">The text area being populated by text.</param>
        /// <param name="endText">How the final text display will look (tags not withstanding).</param>
        /// <returns>War in our time?!?</returns>
        public static async Awaitable RunThroughTextAsync(TMP_Text textBox, string endText, AudioClip dialogSound, float basePitch, float pitchVariance, bool debugTextbox = false)
        {
            textBox.text = "";
            textBox.ForceMeshUpdate();
            while (pausedTextbox) await Awaitable.NextFrameAsync(); //No friggin' idea why this is necessary for first runtime screen?????????

            float t = 0;
            int i = 0; //"real" string char index
            int charI = 0; //tmp_charInfo index (excludes built-in rich tags already... nightmare nightmare parsing!)
            string amended = endText;
            TMP_CharacterInfo curChar;

            TagParser.SetCurrentTextMesh = textBox;

            amended = ReplaceableText(amended);
            amended = Flag_Manager.CleanOutFlags(amended);

            textBox.text = amended; //set the text that will be show over time/mangled horrifically by tags
            textBox.fontSize = PredictFontSize(TagParser.CleanOutTags(amended, false), textBox);
            textBox.ForceMeshUpdate();
            HideLetters(textBox);

            JumpToEndOfTextbox = false;

            string s = PlaceInvisibleCharsBetweenTags(amended, debugTextbox);

            while (charI < textBox.textInfo.characterCount && !JumpToEndOfTextbox && i < textBox.text.Length) //while running through a textbox (player hasn't done nothing)
            {
                //only run through text if unpaused
                while (pausedTextbox) await Awaitable.NextFrameAsync();

                t += Time.deltaTime;
                //add one character at a time, depending on text speed
                if (t >= TextSpeed)
                {
                    t = 0;
                    
                    if (textBox.text[i] == '<') //we've found a rich text tag
                    {
                        //Debug.Log("Touched rich tag?");
                        string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                        while (textBox.text[i] != '>')
                        {
                            i++;
                            if (i >= textBox.text.Length)
                            {
                                Debug.LogError("Tag found that has no closer. Stopping RunThroughTextAsync!");
                                return;
                            }
                            tag += textBox.text[i];
                        }
                        i++;
                        if (TagParser.Parse(tag))
                        {
                            ///Debug.Log("Touched rich tag!");
                            //Because {s} has invisible chars before and after tags for line-spacing concerns, we have to
                            //take that into account when Regex-ing them away.
                            excludeTagEx = new Regex(WSPBreak + tag + WSPBreak);
                            s = excludeTagEx.Replace(s, "", 1);
                            ///if(debugTextbox)Debug.Log($"PredictVerts string: {s}, stringL: {s.Length}");
                            ExcludeLetters(textBox, charI, charI + tag.Length, s, debugTextbox);
                            charI += tag.Length;//?
                        }
                    }
                    if (i >= textBox.text.Length) break;
                    curChar = textBox.textInfo.characterInfo[charI];
                    if (curChar.character != ' ' && curChar.character != '<')
                    {
                        Audio_Manager.AM.Play2DSFX(dialogSound, basePitch - pitchVariance, basePitch + pitchVariance, 1, true, true);
                        if (PlayerPrefs.GetInt("AnimateChars", 1) == 1) _ = FadeInALetterAsync(textBox, charI);
                        else ShowALetter(textBox, charI, debugTextbox);
                        //only care about passing in a stupid char for the event if event actually exists
                        if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                        {
                            try
                            {
                                //pass in the current character if applicable.
                                stu?.Invoke((object)textBox.textInfo.characterInfo[charI]);
                            }
                            catch
                            {
                                Debug.LogWarning("For some stupid reason, Textbox_String is trying to fire the spawn new character event on an invalid indexed character?!? Wild.");
                            }
                        }
                    }
                    charI++;
                    i++;
                }

                await Awaitable.NextFrameAsync();
            }

            JumpToEndOfTextbox = true;

            while (charI < textBox.textInfo.characterCount && i < textBox.text.Length) //just run through everything in a single frame if player skipped ahead.
            {
                if (textBox.text[i] == '<') //we've found a rich text tag
                {
                    string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                    while (textBox.text[i] != '>')
                    {
                        i++;
                        tag += textBox.text[i];
                    }
                    i++;
                    //don't need to bother excluding things if the last letter of tag is the last letter of the string?
                    //might leave a gap big enough to cause smaller text size, but might be preferrable to FURTHER index chicanery
                    if (TagParser.Parse(tag))
                    {
                        excludeTagEx = new Regex(tag);
                        s = excludeTagEx.Replace(s, "", 1);
                        ExcludeLetters(textBox, charI, charI + tag.Length, s, debugTextbox);
                        charI += tag.Length; //
                    }
                }
                if (i >= textBox.text.Length) break;
                curChar = textBox.textInfo.characterInfo[charI];
                if (curChar.character != ' ' && curChar.character != '<')
                {
                    if (PlayerPrefs.GetInt("AnimateChars", 1) == 1) _ = FadeInALetterAsync(textBox, charI);
                    else ShowALetter(textBox, charI, debugTextbox);
                    //only care about passing in a stupid char for the event if event actually exists
                    if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                    {
                        try
                        {
                            //pass in the current character if applicable.
                            stu?.Invoke((object)textBox.textInfo.characterInfo[charI]);
                        }
                        catch
                        {
                            Debug.LogWarning("For some stupid reason, Textbox_String is trying to fire the spawn new character event on an invalid indexed character?!? Wild.");
                        }
                    }
                }
                charI++;
                i++;
            }
        }
        #region MeshManipulationStuff
        /// <summary>
        /// "Hides" every single char in a TMP_Text by setting every single char's vertex colors to be alpha = 0.
        /// </summary>
        /// <param name="textToHide">The text component whose characters you want hidden.</param>
        /// <param name="afterIndex"></param>
        private static void HideLetters(TMP_Text textToHide, int afterIndex = 0, bool debug = false)
        {
            TMP_TextInfo text = textToHide.textInfo;
            TMP_CharacterInfo[] charsToBeHiding = text.characterInfo;
            
            Color32[] vertiColors;
            for(int i = afterIndex; i < text.characterCount; i++)
            {
                int matInd = charsToBeHiding[i].materialReferenceIndex;
                if (text.meshInfo[matInd].colors32 != null)
                {
                    vertiColors = text.meshInfo[matInd].colors32;
                    for (int j = 0; j < vertiColors.Length; j++)
                    {
                        vertiColors[j].a = 0;
                    }
                }
                else
                {
                    vertiColors = new Color32[text.meshInfo[matInd].vertexCount];
                    for(int j = 0; j < vertiColors.Length; j++)
                    {
                        vertiColors[j].r = 255;
                        vertiColors[j].g = 255;
                        vertiColors[j].b = 255;
                        vertiColors[j].a = 0;
                    }
                }
                text.meshInfo[matInd].colors32 = vertiColors;
            }
            textToHide.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            if (!debug) return;
            char[] chars = new char[text.characterCount];
            for (int i = afterIndex; i < chars.Length; i++)
            {
                chars[i] = charsToBeHiding[i].character;
            }
            Debug.Log("Chars that we are hiding: " + chars.ArrayToString());
        }
        /// <summary>
        /// Truncates vertices of characters, making all chars between startI and endI have their vertices set to Vec3.zero,
        ///  while every character after endI (including endI) having their vertices set to a copy mesh that has the excluded
        ///   characters removed.
        /// </summary>
        /// <param name="text">The textbox you want affected.</param>
        /// <param name="startI">The (inclusive) starting index of characters you want hidden entirely.</param>
        /// <param name="endI">The (exclusive) ending index of characters you want hidden. Is the start of the truncation.</param>
        /// <param name="expectedText">The text you expect the textbox to look like when done (the textbox if it's tmp.text didn't
        ///  have the characters between startI and endI.)</param>
        /// <param name="debug">lol</param>
        private static void ExcludeLetters(TMP_Text text, int startI, int endI, string expectedText, bool debug = false)
        {
            //works when first char of string is rich tag. but not second?
            TMP_CharacterInfo[] chars = text.textInfo.characterInfo;
            TMP_CharacterInfo charToExclude;
            //int matInd,
            int vertInd;
            Mesh goose = text.mesh;
            Vector3[] verts = goose.vertices;
            string s = "";
            //do the hiding part
            for(int i = startI; i < endI; i++)
            {
                charToExclude = chars[i];
                s += charToExclude.character;
                vertInd = charToExclude.vertexIndex;
                
                for (int j = vertInd; j < vertInd + 4; j++)
                {
                    verts[j] = Vector3.zero;
                }
            }
            if(debug) Debug.Log("Excluding chars: " + s + $", stringL: {s.Length}");
            //vertices for a textbox that excludes the stinkin' letters
            //don't bother spending cpu adjusting further letters if endI is last letter.
            if (endI == text.text.Length) 
            { 
                Debug.Log("Found tag is end of string. Not truncating.");
                goose.vertices = verts;
                ///text.mesh.vertices = verts;
                text.canvasRenderer.SetMesh(goose);
                return; 
            } 
            Vector3[] expectedVerts = PredictVertices(expectedText, text, false, debug);
            //we need our vertex gap math to ignore the god forsaken \u200b hogwash, even if we need to include it in the predict verts mesh.
            int vertexGap = (text.textInfo.characterCount - TagParser.CleanedOutBuiltInTagsLength(Regex.Replace(expectedText,WSPBreak,""))) * 4;

            for(int i = endI; i < text.textInfo.characterCount; i++)
            {
                try
                {
                    //if we don't skip spaces, sometimes letters show up where we don't want them.
                    //fairly certain this is because how vertices are assigned in memory for spaces involves BS chicanery that
                    //links them to other characters (because spaces don't need to be rendered at all, even if we need to keep track
                    //of them)
                    if (chars[i].character == ' ') continue; 
                    vertInd = chars[i].vertexIndex; //characterVertex index is not equivalent to i + n. Byte-Batching-BS istg
                    
                    for (int j = vertInd; j < vertInd + 4; j++)
                    {
                        verts[j] = expectedVerts[j - vertexGap];
                    }
                }
                catch
                {  
                    Debug.LogWarning("Problem in Textbox_String.cs ExcludeLetters()'s downline vertex adjustment. Continuing.");
                    continue;
                }
            }
            //mesh setting because meshInfo[] is really dumb and bad and I hate it! (overlays are overrated anyway... probably.)
            goose.vertices = verts;
            ///text.mesh.vertices = verts;
            text.canvasRenderer.SetMesh(goose);
        }
        /// <summary>
        /// Based on a given string and TMP_Text settings, predict the vertices of the text mesh. 
        /// </summary>
        /// <param name="toPredict">The text whose vertices you wish to predict.</param>
        /// <param name="settings">The TMP_Text whose settings you want taken into account.</param>
        /// <param name="TMPType">False for TextMeshProUGUI, True for TextMeshPro</param>
        /// <returns>The predicted string's mesh's vertices array.</returns>
        private static Vector3[] PredictVertices(string toPredict, TMP_Text settings, bool TMPType = false, bool debug = false)
        {
            //i'm pretty sure the reason we need an existent GO is because TMPUGUI forcemeshupdate doesn't do squat on "floating"
            //components. also it won't work on objects that were literally just constructed, so we use an empty object already
            //spawned by Sequence_Manager on Awake/LoadGame.
            if(!Object_Factory.TryGetObject("EMPTY", out GameObject temp))
            {
                Debug.LogError("Textbox_String couldn't find the 'EMPTY' gameobject to test out a text mesh with. Can't predict" +
                    " vertices with nothing, returning an empty array.");
                return new Vector3[0];
            }
            if (!temp.TryGetComponent(out TMP_Text ttmp))
            {
                //add component if we don't already have one (it'll be destroyed by end, so idk man)
                ttmp = !TMPType ? temp.AddComponent<TextMeshProUGUI>() : temp.AddComponent<TextMeshPro>();
            }
            ttmp.CopyTMPSettings(settings);
            ttmp.text = toPredict;
            ttmp.ForceMeshUpdate(); //guh...
            Vector3[] curMeshVerts = ttmp.mesh.vertices;
            if (debug)
            {
                Debug.LogWarning("Text: '" + ttmp.text + "', textinfo chr count: " + ttmp.textInfo.characterCount + 
                    ", textLength " + ttmp.text.Length + ", textinfo chrArrayLength " + ttmp.textInfo.characterInfo.Length + 
                    $", curMeshVertsL: {curMeshVerts.Length}");
                ttmp.fontStyle = FontStyles.Underline;
                ttmp.color = CoolColors.slightTransYellowGUI;
            }
            //this line used to be Destroy(ttmp). for some ungodly reason, even though the verts have already been gotten,
            //it doesn't work whenever it happens all in one frame? (player sets jumpToEnd true before coroutine does.)
            //now, just make it invisible.
            if (!debug) ttmp.color = CoolColors.transparent;
            return curMeshVerts;
        }
        /// <summary>
        /// Based on a given string and TMP_Text settings, predict the automatic font size of the text mesh.
        /// </summary>
        /// <param name="toPredict">The text whose font size you wish to predict.</param>
        /// <param name="settings">The TMP_Text whose settings you want taken into account.</param>
        /// <param name="TMPType">False for TextMeshProUGUI, True for TextMeshPro</param>
        /// <returns>The predicted string's TMP auto font size.</returns>
        private static float PredictFontSize(string toPredict, TMP_Text settings, bool TMPType = false)
        {
            if(!Object_Factory.TryGetObject("EMPTY", out GameObject temp))
            {
                Debug.LogError("Textbox_String couldn't find the 'EMPTY' gameobject to test out a text mesh with. Can't predict " +
                    "appropriate font size with nothing, returning the current font size.");
                return settings.fontSize;
            }
            if(!temp.TryGetComponent(out TMP_Text ttmp))
            {
                ttmp = !TMPType ? temp.AddComponent<TextMeshProUGUI>() : temp.AddComponent<TextMeshPro>();
            }
            ttmp.CopyTMPSettings(settings);
            ttmp.text = toPredict;
            ttmp.enableAutoSizing = true;
            ttmp.ForceMeshUpdate();
            float foundFontSize = ttmp.GetCurrentFontSize();
            UObject.Destroy(ttmp);
            return foundFontSize;
        }
        /// <summary>
        /// Sets a single letter to have full alpha vertex colors.
        /// </summary>
        /// <param name="text">The textbox you want to affect.</param>
        /// <param name="i">The index of the character (textInfo.characterInfo, not raw text string) you want to show.</param>
        /// <param name="debug"></param>
        private static void ShowALetter(TMP_Text text, int i, bool debug = false)
        {
            try
            {
                TMP_TextInfo textInfo = text.textInfo;
                TMP_CharacterInfo charToShow = textInfo.characterInfo[i];
                Color32[] vertiColors = textInfo.meshInfo[charToShow.materialReferenceIndex].colors32;
                for (int j = charToShow.vertexIndex; j < charToShow.vertexIndex + 4; j++)
                {
                    vertiColors[j].a = 255;
                }
                textInfo.meshInfo[charToShow.materialReferenceIndex].colors32 = vertiColors;
                text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                if (!debug) return;
                Vector3[] verts = text.mesh.vertices;
                Debug.Log("Char to show: " + charToShow.character + $" Vertices: {verts[charToShow.vertexIndex]}," +
                    $"{verts[charToShow.vertexIndex + 1]},{verts[charToShow.vertexIndex + 2]},{verts[charToShow.vertexIndex+3]}");
            }
            catch
            {
                Debug.LogWarning($"Bad Char Given, no showing :(");
            }
        }
        /// <summary>
        /// Sets a single letter to have full alpha vertex colors over time.
        /// </summary>
        /// <param name="text">The textbox you want to affect.</param>
        /// <param name="i">The index of the character (textInfo.characterInfo, not raw text string) you want to show.</param>
        /// <param name="debug"></param>
        /// <returns>A visible character.</returns>
        private static async Awaitable FadeInALetterAsync(TMP_Text text, int i, bool debug = false)
        {
            float t = 0;
            TMP_CharacterInfo charToShow = text.textInfo.characterInfo[i];
            Color32[] vertiColors = text.textInfo.meshInfo[charToShow.materialReferenceIndex].colors32;
            byte c;
            
            if (debug)
            {
                Vector3[] verts = text.textInfo.meshInfo[charToShow.materialReferenceIndex].vertices;
                Debug.Log("Char to show: " + charToShow.character + $" Vertices: {verts[charToShow.vertexIndex]}," +
                    $"{verts[charToShow.vertexIndex + 1]},{verts[charToShow.vertexIndex + 2]},{verts[charToShow.vertexIndex + 3]}");
            }
            while (t < FadeCharSpeed && vertiColors[charToShow.vertexIndex].a != 255 && !JumpToEndOfTextbox)
            {
                vertiColors = text.textInfo.meshInfo[charToShow.materialReferenceIndex].colors32;

                c = TrigHelper.PercentToByte(characterFadeInCurve.Evaluate(t / FadeCharSpeed));
                for (int j = charToShow.vertexIndex; j < charToShow.vertexIndex + 4; j++)
                {
                    vertiColors[j].a = c;
                }

                t += Time.deltaTime;
                text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                await Awaitable.NextFrameAsync();
            }
            if (vertiColors[charToShow.vertexIndex].a == 255) return;
            for (int j = charToShow.vertexIndex; j < charToShow.vertexIndex + 4; j++)
            {
                vertiColors[j].a = 255;
            }
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
        #endregion
        /// <summary>
        /// Places invisible characters (\u200B) before every instance of "greater-than" and after every instance of "less-than."
        /// Useful for predicting vertices whenever line-break spacing gets annoying. (TMP will interpret a rich tag surrounding
        ///  a single word as all one big word, which is unideal and can cause vertices to appear on the wrong line of text.)
        /// </summary>
        /// <param name="sInput">The text you want to put invisible characters into.</param>
        /// <param name="debug"></param>
        /// <returns>The given string with invisible characters (\u200B) before and after tags.</returns>
        public static string PlaceInvisibleCharsBetweenTags(string sInput, bool debug)
        {
            int i = 0;
            string toReturn = sInput;
            while (i < toReturn.Length)
            {
                if (toReturn[i] == '<') //insert whitespace before <
                {
                    toReturn = toReturn.Insert(i, WSPBreak);
                    i++; 
                } 
                else if (toReturn[i] == '>') //insert whitespace after >
                {
                    i++;
                    toReturn = toReturn.Insert(i, WSPBreak);
                }
                i++;
            }
            if (debug) Debug.LogWarning($"InvisChars: '{toReturn}', string length: {toReturn.Length}, original length: {sInput.Length}");
            return toReturn;
        }
        /// <summary>
        /// Goes through a string, and replaces all instances of keys in the replaceabletexts dictionary with their values.
        /// Ie. Replace {mc} with the name stored in savedata, etc.
        /// </summary>
        /// <param name="sInput">The text you want parsed.</param>
        /// <returns>The input with all replaceable texts replaced.</returns>
        public static string ReplaceableText(string sInput)
        {
            foreach (KeyValuePair<string, string> kvp in replacerTexts) //by the end of this, replace generic guys like {mc} with the actual player's name
            {
                //Debug.Log("Doing a stupid replacering!");
                if (sInput.Contains(kvp.Key))
                {
                    sInput = Regex.Replace(sInput, kvp.Key, kvp.Value);
                }
            }
            return sInput;
        }

        /// <summary>
        /// Stops any textbox from displaying new chars, hover on current string instead. Technically different from pausing
        /// the Sequence.
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
                Debug.Log($"Adding replacer text to dictionary. {replaced} will be replaced by: {replacer}");
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
            if(replaced.Length != replacers.Length)
            {
                Debug.LogError("Somehow, Textbox_String received a replacer array whose size doesn't match the corresponding replaced array." +
                    " Either Save/Load is borked in some manner, or you're feeding it some garbage that won't make a proper dict. Returning.");
                return;
            }
            replacerTexts = new();
            //replaced[] and replacers[] SHOULD be the same length, since they're saved into file by grabbing from the dict
            for (int i = 0; i < replaced.Length; i++)
            {
                replacerTexts.Add(replaced[i], replacers[i]);
            }
        }
    }
}
