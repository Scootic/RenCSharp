using System.Collections;
using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using RenCSharp.Tags;
using RenCSharp.EXPERIMENTAL;
using UObject = UnityEngine.Object;
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
        /// Regex used when excluding custom rich-tags from being rendered in-game. Declared in class to do less GC.
        /// </summary>
        private static Regex excludeTagEx;
        /// <summary>
        /// Does animated text for a textbox, one character at a time. can be forced to fill in every other character by setting
        /// JumpToEndOfTextbox to true. Idles on a character if pausedTextbox is true.
        /// </summary>
        /// <param name="textBox">The text area being populated by the text.</param>
        /// <param name="endText">The stupid dumb string that the textbox will be filled with</param>
        /// <returns>Peace in our time.</returns>
        public static IEnumerator RunThroughText(TMP_Text textBox, string endText)
        {
            while (pausedTextbox) yield return null; //No friggin' idea why this is necessary for first runtime screen?????????

            float t = 0;
            int i = 0; //"real" string char index
            int charI = 0; //tmp_charInfo index (excludes built-in rich tags already... nightmare nightmare parsing!)
            string amended = endText;
            
            TagParser.SetCurrentTextMesh = textBox;

            amended = ReplaceableText(amended);
            amended = Flag_Manager.CleanOutFlags(amended);

            textBox.text = amended; //set the text that will be show over time/mangled horrifically by tags
            textBox.ForceMeshUpdate();
            HideLetters(textBox);

            JumpToEndOfTextbox = false;

            string s = amended;

            while (charI < textBox.textInfo.characterCount && !JumpToEndOfTextbox && i < textBox.text.Length) //while running through a textbox (player hasn't done nothing)
            {
                //only run through text if unpaused
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
                        //Debug.Log("Touched rich tag?");
                        string tag = "" + textBox.text[i]; //collect all the chars that make up our tag
                        while (textBox.text[i] != '>')
                        {
                            i++;
                            tag += textBox.text[i];
                        }

                        if (TagParser.Parse(tag))
                        {
                            Debug.Log("Touched rich tag!");
                            excludeTagEx = new Regex(tag);
                            s = excludeTagEx.Replace(s, "", 1);
                            i++;
                            ExcludeLetters(textBox, charI, i, s);
                            charI += tag.Length;//?
                        }
                    }
                    else //just add the char and move on if it's a regular ah character
                    {
                        ShowALetter(textBox, charI);
                        //only care about passing in a stupid char for the event if event actually exists
                        if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                        {
                            try
                            {
                                //pass in the current character if applicable.
                                Event_Bus.TryFireSingleObjEvent("TextboxNewChar", (object)textBox.textInfo.characterInfo[charI]);
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
                    if (TagParser.Parse(tag))
                    {
                        excludeTagEx = new Regex(tag);
                        s = excludeTagEx.Replace(s, "", 1);
                        i++;
                        ExcludeLetters(textBox, charI, i, s);
                        charI += tag.Length; //
                    }
                }
                else //just add the char and move on if it's a regular ah character
                {
                    //textBox.maxVisibleCharacters++;
                    ShowALetter(textBox, charI);
                    //only care about passing in a stupid char for the event if event actually exists
                    if (Event_Bus.TryGetSingleObjEvent("TextboxNewChar", out Action<object> stu))
                    {
                        stu.Invoke((object)textBox.textInfo.characterInfo[charI]);
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
        public static async Awaitable RunThroughTextAsync(TMP_Text textBox, string endText)
        {
            while (pausedTextbox) await Awaitable.NextFrameAsync(); //No friggin' idea why this is necessary for first runtime screen?????????

            float t = 0;
            int i = 0; //"real" string char index
            int charI = 0; //tmp_charInfo index (excludes built-in rich tags already... nightmare parsing)
            string amended = endText;

            TagParser.SetCurrentTextMesh = textBox;

            amended = ReplaceableText(amended);
            amended = Flag_Manager.CleanOutFlags(amended);

            textBox.text = amended; //set the text that will be show over time/mangled horrifically by tags
            textBox.ForceMeshUpdate();
            HideLetters(textBox);

            JumpToEndOfTextbox = false;

            string s = textBox.text;

            while (charI < textBox.textInfo.characterCount && !JumpToEndOfTextbox && i < textBox.text.Length) //while running through a textbox (player hasn't done nothing)
            {
                //only run through text if the SM is unpaused
                while (pausedTextbox)
                {
                    await Awaitable.NextFrameAsync();
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

                        if (TagParser.Parse(tag))
                        {
                            excludeTagEx = new Regex(tag);
                            s = excludeTagEx.Replace(s, "", 1);
                            //textBox.text = s;
                            //textBox.ForceMeshUpdate(true);
                            //i -= tag.Length - 1;
                            //HideLetters(textBox,charI);
                            i++;
                            ExcludeLetters(textBox, charI, i, s);
                            //charI = Mathf.Min(charI + tag.Length, textBox.textInfo.characterCount - 1);
                            charI += tag.Length;//?
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
                    //don't need to bother excluding things if the last letter of tag is the last letter of the string?
                    //might leave a gap big enough to cause smaller text size, but might be preferrable to FURTHER index chicanery
                    if (TagParser.Parse(tag))
                    {
                        excludeTagEx = new Regex(tag);
                        s = excludeTagEx.Replace(s, "", 1);
                        //textBox.text = s;
                        //textBox.ForceMeshUpdate(true);
                        //i -= tag.Length - 1;
                        //HideLetters(textBox,charI);
                        i++;
                        ExcludeLetters(textBox, charI, i, s);
                        //charI = Mathf.Min(charI + tag.Length, textBox.textInfo.characterCount - 1);
                        charI += tag.Length; //

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
        /// "Hides" every single char in a TMP_Text by setting every single char's vertex colors to be alpha = 0.
        /// </summary>
        /// <param name="textToHide">The text component whose characters you want hidden.</param>
        /// <param name="afterIndex"></param>
        private static void HideLetters(TMP_Text textToHide, int afterIndex = 0)
        {
            TMP_TextInfo text = textToHide.textInfo;
            TMP_CharacterInfo[] charsToBeHiding = text.characterInfo;
            /*
            char[] chars = new char[text.characterCount];
            for (int i = afterIndex; i < chars.Length; i++)
            {
                chars[i] = charsToBeHiding[i].character;
            }
            Debug.Log("Chars that we are hiding: " + chars.ArrayToString());
            */ 
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

        private static void ExcludeLetters(TMP_Text text, int startI, int endI, string expectedText)
        {
            //works when first char of string is rich tag. but not second?
            TMP_CharacterInfo[] chars = text.textInfo.characterInfo;
            TMP_CharacterInfo charToExclude;
            int matInd, vertInd;
            Vector3[] verts = new Vector3[0];
            string s = "";
            //do the hiding part
            for(int i = startI; i < endI; i++)
            {
                charToExclude = chars[i];
                s += charToExclude.character;
                matInd = charToExclude.materialReferenceIndex;
                vertInd = charToExclude.vertexIndex;
                verts = text.textInfo.meshInfo[matInd].vertices;
                
                for (int j = vertInd; j < vertInd + 4; j++)
                {
                    verts[j] = Vector3.zero;
                }

                text.textInfo.meshInfo[matInd].vertices = verts; //why is meshInfo an array like this...
            }
            Debug.Log("Excluding chars: " + s);
            //vertices for a textbox that excludes the stinkin' letters
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            if (endI == text.text.Length) { Debug.Log("Found tag is end of string. Not truncating."); return; } //don't bother spending cpu adjusting further letters if endI is last letter.
            Vector3[] expectedVerts = PredictVertices(expectedText, text);

            //Debug.Log($"Current first mesh verts?: {verts[0]},{verts[1]},{verts[2]},{verts[3]}. Mesh VCount: {verts.Length}");
            //Debug.Log($"Expected first mesh verts: {expectedVerts[0]},{expectedVerts[1]},{expectedVerts[2]},{expectedVerts[3]}. Mesh VCount: {expectedVerts.Length}");
            //I HAVE GENUINELY NO IDEA WHY YOU NEED TO DO ENDI + 1 FOR IT TO WORK?!?!?

            //starting at endI itself means that the verts taken will be offset by one char to the right (The start of the sentence
            //would be set to the second letter position, and so on and so forth).
            //It's extra weird because the hiding method ends before it reaches endI, so no one ever touches it?!?
            //TMP IS SO FRIGGIN' CRAZY
            string s2 = "";
            string s3 = "";
            int vertexGap = (text.textInfo.characterCount - TagParser.CleanedOutBuiltInTagsLength(expectedText)) * 4;
            //when rich tag is after start, last char takes on the verts of the first...?
            for(int i = endI; i < text.textInfo.characterCount; i++)
            {
                try
                {
                    vertInd = chars[i].vertexIndex; //the vertInd isn't fully linear...????????????
                    s2 += chars[i].character;
                    for (int j = vertInd; j < vertInd + 4; j++)
                    {
                        //if((j - firstVertInd) < 0) { firstVertInd = 0; } //if, for some god awful reason, the vertices are split like a total bith.
                    s3 += ($"Char: {chars[i].character}, VertexInd: " + vertInd + $", (ind{j} - vertexGap): " + (j - vertexGap) + ", expectedVertsL: " + expectedVerts.Length + ", vertsL: " + verts.Length + "\n");

                    //if (!(vertInd < firstVertInd)) 
                        verts[j] = expectedVerts[j - vertexGap];
                        ///verts[j] = expectedVerts[(i - endI) * 4 + (j - vertInd)];
                    //else verts[j] = expectedVerts[j];
                    }
                }
                catch
                {
                    //occurs twice for when the rich tag is the first part of string
                    //doesn't occur when the rich tag is after 
                    Debug.LogWarning("Problem in Textbox_String.cs ExcludeLetters()'s downline vertex adjustment. Continuing.");
                    continue;
                }
            }
            Debug.Log(s3);
            Debug.Log("Characters being truncated: " + s2);
            //mesh setting because meshInfo[] is really dumb and bad and I hate it! (overlays are overrated anyway... probably.)
            Mesh goose = text.mesh;
            goose.vertices = verts;
            text.canvasRenderer.SetMesh(goose);
        }
        /// <summary>
        /// Based on a given string and TMP_Text settings, predict the vertices of the text mesh. 
        /// </summary>
        /// <param name="toPredict">The text whose vertices you wish to predict.</param>
        /// <param name="settings">The TMP_Text whose settings you want taken into account.</param>
        /// <param name="TMPType">False for TextMeshProUGUI, True for TextMeshPro</param>
        /// <returns>The predicted string's mesh's vertices array.</returns>
        private static Vector3[] PredictVertices(string toPredict, TMP_Text settings, bool TMPType = false)
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
                ttmp = !TMPType ? temp.AddComponent<TextMeshProUGUI>() : temp.AddComponent<TextMeshPro>();
            }
            ttmp.CopyTMPSettings(settings);
            ttmp.text = toPredict;
            ttmp.ForceMeshUpdate(); //guh...
            Debug.LogWarning("Text: '" + ttmp.text + "', textinfo chr count: " + ttmp.textInfo.characterCount + ", textLength " + ttmp.text.Length + ", textinfo chrArrayLength " + ttmp.textInfo.characterInfo.Length);
            
            Vector3[] expectedVerts;
            expectedVerts = ttmp.mesh.vertices;
            UObject.DestroyImmediate(ttmp);
            return expectedVerts;
        }

        private static void ShowALetter(TMP_Text text, int i)
        {
            try
            {
                TMP_CharacterInfo charToShow = text.textInfo.characterInfo[i];
                //Debug.Log("Char to show: " + charToShow.character + " Index: " + i);

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
