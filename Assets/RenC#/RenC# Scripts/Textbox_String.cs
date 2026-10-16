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
        /// inputted name given by player. Used by the RunThroughText coroutine. Certified Regex moment.
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
            while (pausedTextbox) yield return null; //No friggin' idea why this is necessary for first runtime screen?????????

            float t = 0;
            int i = 0; //"real" string char index
            int charI = 0; //tmp_charInfo index (excludes built-in rich tags already... nightmare parsing)
            string amended = endText;
            string s = "";
            TagParser.SetCurrentTextMesh = textBox;

            amended = ReplaceableText(amended);
            amended = TagParser.CleanOutFlags(amended);

            textBox.text = amended; //set the text that will be show over time/mangled horrifically by tags
            textBox.ForceMeshUpdate();
            HideLetters(textBox);

            JumpToEndOfTextbox = false;

            while (charI < textBox.textInfo.characterCount && !JumpToEndOfTextbox && i < textBox.text.Length) //while running through a textbox (player hasn't done nothing)
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
                        Debug.Log("Touched rich tag?");
                        string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                        while (textBox.text[i] != '>')
                        {
                            i++;
                            tag += textBox.text[i];
                        }

                        if (TagParser.Parse(tag) && i != textBox.text.Length - 1)
                        {
                            s = s == "" ? Regex.Replace(textBox.text, tag, "") : Regex.Replace(s, tag, "");
                            //textBox.text = s;
                            //textBox.ForceMeshUpdate(true);
                            //i -= tag.Length - 1;
                            //HideLetters(textBox,charI);
                            
                            ExcludeLetters(textBox, charI, i, s);
                            //charI = Mathf.Min(charI + tag.Length, textBox.textInfo.characterCount - 1);
                            charI += tag.Length - 1;//?
                            //i = Mathf.Min(i + tag.Length, textBox.text.Length);
                            //if (charI != 0) charI -= tag.Length - 1;

                        }
                        //charI -= tag.Length - 1;
                    }
                    else //just add the char and move on if it's a regular ah character
                    {
                        ShowALetter(textBox, charI);
                        //only care about passing in a stupid char for the event if event actually exists
                        if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                        {
                            try
                            {
                                TMP_CharacterInfo c = textBox.textInfo.characterInfo[charI];
                                Event_Bus.TryFireSingleObjEvent("TextboxNewChar", (object)c);
                            }
                            catch
                            {
                                Debug.LogWarning("For some stupid reason, Textbox_String is trying to fire the spawn new character event on an invalid indexed character?!? Wild.");
                            }
                        }
                        charI++;
                        i++;
                    }
                }

                yield return null;
            }

            JumpToEndOfTextbox = true;

            while(charI < textBox.textInfo.characterCount && i < textBox.text.Length) //just run through everything in a single frame if player skipped ahead.
            {
                if (textBox.text[i] == '<') //we've found a rich text tag
                {
                    string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                    while (textBox.text[i] != '>')
                    {
                        i++;
                        tag += textBox.text[i];
                    }
                    //don't need to bother excluding things if the last letter of tag is the last letter of the string?
                    //might leave a gap big enough to cause smaller text size, but might be preferrable to FURTHER index chicanery
                    if (TagParser.Parse(tag) && i != textBox.text.Length - 1)
                    {
                        s = s == "" ? Regex.Replace(textBox.text, tag, "") : Regex.Replace(s, tag, "");
                        //textBox.text = s;
                        //textBox.ForceMeshUpdate(true);
                        //i -= tag.Length - 1;
                        //HideLetters(textBox,charI);

                        ExcludeLetters(textBox, charI, i, s);
                        //charI = Mathf.Min(charI + tag.Length, textBox.textInfo.characterCount - 1);
                        charI += tag.Length - 1;
                        //i = Mathf.Min(i + tag.Length, textBox.text.Length);
                        //if (charI != 0) charI -= tag.Length - 1;

                    }
                }
                else //just add the char and move on if it's a regular ah character
                {
                    //textBox.maxVisibleCharacters++;
                    ShowALetter(textBox, charI);
                    //only care about passing in a stupid char for the event if event actually exists
                    if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                    {
                        TMP_CharacterInfo c = textBox.textInfo.characterInfo[charI];
                        stu.Invoke((object)c);
                    }
                    i++;
                    charI++;
                }
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
        public static async Awaitable RunThroughTextAsync(TextMeshProUGUI textBox, string endText)
        {
            while (pausedTextbox) await Awaitable.NextFrameAsync();
            float t = 0;
            int i = 0;
            int charI = 0;
            string amended = endText;
            TagParser.SetCurrentTextMesh = textBox;

            amended = ReplaceableText(amended);
            amended = TagParser.CleanOutFlags(amended);

            textBox.text = amended; //insert text that will be shown over time

            //textBox.maxVisibleCharacters = 0;
            textBox.ForceMeshUpdate();
            HideLetters(textBox);

            
            JumpToEndOfTextbox = false;

            while (charI < textBox.textInfo.characterCount && !JumpToEndOfTextbox) //while running through a textbox (player hasn't done nothing)
            {
                //only run through text if the SM is unpaused
                while (pausedTextbox) await Awaitable.NextFrameAsync();
  
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
                            //textBox.textInfo.characterInfo[i].isVisible = false;
                            i++;
                            tag += textBox.text[i];
                        }

                        if (TagParser.Parse(tag))
                        {
                            string s = Regex.Replace(textBox.text, tag, "");
                            textBox.text = s;
                            textBox.ForceMeshUpdate(true);
                            i -= tag.Length - 1;
                            if(charI != 0)charI -= tag.Length - 1;
                            HideLetters(textBox, charI);
                        }
                        //charI -= tag.Length - 1;
                    }
                    else //just add the char and move on if it's a regular ah character
                    {
                        //textBox.maxVisibleCharacters++;
                        ShowALetter(textBox, charI);
                        ///textBox.ForceMeshUpdate();
                        //only care about passing in a stupid char for the event if event actually exists
                        if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                        {
                            ///int goodI = TagParser.StringIndexExcludeBuiltinTags(textBox.text, i, true); //since the text string gets rid of valid tags???
                            try
                            {
                                TMP_CharacterInfo c = textBox.textInfo.characterInfo[charI];
                                Event_Bus.TryFireSingleObjEvent("TextboxNewChar", (object)c);
                            }
                            catch
                            {
                                Debug.LogWarning("For some stupid reason, Textbox_String is trying to fire the spawn new character event on an invalid indexed character?!? Wild.");
                            }
                        }
                        charI++;
                        i++;
                    }
                }

                await Awaitable.NextFrameAsync();
            }

            JumpToEndOfTextbox = true;

            while (charI < textBox.textInfo.characterCount) //just run through everything in a single frame if player skipped ahead.
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
                        textBox.ForceMeshUpdate();
                        i -= tag.Length - 1; //??
                        if(charI != 0)charI -= tag.Length - 1;
                    }
                }
                else //just add the char and move on if it's a regular ah character
                {
                    //textBox.maxVisibleCharacters++;
                    ShowALetter(textBox, charI);
                    //only care about passing in a stupid char for the event if event actually exists
                    if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                    {
                        ///int goodI = TagParser.StringIndexExcludeBuiltinTags(textBox.text, i);
                        TMP_CharacterInfo c = textBox.textInfo.characterInfo[charI];
                        stu.Invoke((object)c);
                    }
                    i++;
                    charI++;
                }
            }
            textBox.ForceMeshUpdate();
            //textBox.maxVisibleCharacters = textBox.text.Length; //please show everything if we aren't already!
        }

        private static void HideLetters(TextMeshProUGUI textToHide, int afterIndex = 0)
        {
            TMP_TextInfo text = textToHide.textInfo;
            TMP_CharacterInfo[] charsToBeHiding = text.characterInfo;
            char[] chars = new char[text.characterCount];
            for (int i = afterIndex; i < chars.Length; i++)
            {
                chars[i] = charsToBeHiding[i].character;
            }
            Debug.Log("Chars that we are hiding: " + chars.ArrayToString());
               
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
        }

        private static void ExcludeLetters(TextMeshProUGUI text, int startI, int endI, string expectedText)
        {
            TMP_CharacterInfo[] chars = text.textInfo.characterInfo;
            TMP_CharacterInfo charToExclude;
            int matInd, vertInd;
            Vector3[] verts = new Vector3[0];
                //do the hiding part
            for(int i = startI; i < endI; i++)
            {
                charToExclude = chars[i];
                matInd = charToExclude.materialReferenceIndex;
                vertInd = charToExclude.vertexIndex;
                verts = text.textInfo.meshInfo[matInd].vertices;

                for (int j = vertInd; j < vertInd + 4; j++)
                {
                    verts[j] = verts[vertInd];
                }

                text.textInfo.meshInfo[matInd].vertices = verts; //why is meshInfo an array like this...
            }
                //vertices for a textbox that excludes the stinkin' letters
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            Vector3[] expectedVerts = PredictVertices(expectedText, text);

            Debug.Log($"Current first mesh verts?: {verts[0]},{verts[1]},{verts[2]},{verts[3]}. Mesh VCount: {verts.Length}");
            Debug.Log($"Expected first mesh verts: {expectedVerts[0]},{expectedVerts[1]},{expectedVerts[2]},{expectedVerts[3]}. Mesh VCount: {expectedVerts.Length}");

            for(int i = 0; i < expectedVerts.Length; i++)
            {
                //endI * 4 because endI is a char, not a vert...?
                try
                {
                    verts[i + (endI * 4)] = expectedVerts[i];
                }
                catch
                {
                    continue;
                }
            }
            //mesh setting because meshInfo[] is really dumb and bad and I hate it! (overlays are overrated anyway... probably.)
            Mesh goose = text.mesh;
            goose.vertices = verts;
            text.canvasRenderer.SetMesh(goose);
        }

        private static Vector3[] PredictVertices(string toPredict, TextMeshProUGUI settings)
        {
            //i'm pretty sure the reason we need an existant GO is because TMPUGUI forcemeshupdate doesn't do squat on "floating"
            //components. also it won't work on objects that were literally just constructed, so we use an empty object already
            //spawned by Sequence_Manager on Awake/LoadGame.
            Object_Factory.TryGetObject("EMPTY", out GameObject temp);
            TextMeshProUGUI ttmp = temp.AddComponent<TextMeshProUGUI>();
            ttmp.CopyTMPSettings(settings);
            ttmp.text = toPredict;
            ttmp.ForceMeshUpdate(); //guh...
            Debug.LogWarning("Text: " + ttmp.text + ", textinfo chr count: " + ttmp.textInfo.characterCount + ", textLength " + ttmp.text.Length + ", textinfo chrArrayLength " + ttmp.textInfo.characterInfo.Length);

            Vector3[] expectedVerts = new Vector3[ttmp.textInfo.characterCount * 4];
            TMP_CharacterInfo tci;
            int vertI;
            for (int i = 0; i < ttmp.textInfo.characterCount; i++)
            {
                tci = ttmp.textInfo.characterInfo[i];
                vertI = i * 4;
                //struct garbage is why we can't nested for... D; so sad
                expectedVerts[vertI] = tci.bottomLeft;
                expectedVerts[vertI + 1] = tci.topLeft;
                expectedVerts[vertI + 2] = tci.topRight;
                expectedVerts[vertI + 3] = tci.bottomRight;
            }

            GameObject.Destroy(ttmp);
            return expectedVerts;
        }

        private static void ShowALetter(TextMeshProUGUI text, int i)
        {
            try
            {
                TMP_CharacterInfo charToShow = text.textInfo.characterInfo[i];
                Debug.Log("Char to show: " + charToShow.character + " Index: " + i);

                TMP_TextInfo textInfo = text.textInfo;

                Color32[] vertiColors = textInfo.meshInfo[charToShow.materialReferenceIndex].colors32;
                for (int j = charToShow.vertexIndex; j < charToShow.vertexIndex + 4; j++)
                {
                    vertiColors[j].a = 255;
                }
                textInfo.meshInfo[charToShow.materialReferenceIndex].colors32 = vertiColors;
                text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }
            catch
            {
                Debug.LogWarning($"Bad Char Index! Index: {i}");
            }
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
                if (sOutput.Contains(kvp.Key))
                {
                    sOutput = Regex.Replace(sOutput, kvp.Key, kvp.Value);
                }
            }
            return sOutput;
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
            replacerTexts = new();
            //replaced[] and replacers[] SHOULD be the same length, since they're saved into file by grabbing from the dict
            for (int i = 0; i < replaced.Length; i++)
            {
                replacerTexts.Add(replaced[i], replacers[i]);
            }
        }
    }
}
