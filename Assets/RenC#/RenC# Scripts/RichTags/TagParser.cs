using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
namespace RenCSharp.Tags
{
    public class TagParser
    {
        private static readonly Assembly tagAssembly = Assembly.GetAssembly(typeof(TagParser));
        private static readonly Type[] allTags = tagAssembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(Base_Tag))).ToArray();
        private static readonly TagParser instance = new();
        private static TMP_Text currentTextMesh;

        public static TMP_Text SetCurrentTextMesh { set { currentTextMesh = value; } }

        /// <summary>
        /// Find if we have ourselves a valid tag parser tag. (Custom rich tag.) Fire that mf's functionality if we do.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>True if we found a valid (non-built-in, like italicize) tag, false otherwise.</returns>
        public static bool Parse(string tag, bool fire = true)
        {
            if (TagRoutineHandler.TRH.Debug) Debug.Log("Length of valid tag types: " + allTags.Length);
            string[] split = Regex.Split(tag, "[=,]"); //0 should be function name, 1+ is arguments
            List<object> splitNoFirst = new();
            for (int i = 1; i < split.Length; i++)
            {
                splitNoFirst.Add(split[i]);
            }

            split[0] = Regex.Replace(split[0], "/", "End"); //swap out the slash symbol for "End" which is what it represents
            split[0] = Regex.Replace(split[0], "[<>]", ""); //get rid of tag wrapper symbols

            for (int i = 0; i < splitNoFirst.Count; i++)
            {
                splitNoFirst[i] = Regex.Replace((string)splitNoFirst[i], "[<>]", "");
            }

            if (!split[0].Contains("End")) splitNoFirst.Insert(0, currentTextMesh); //if it's an "End" function, we can assume no arguments are being passed in

            if (TagRoutineHandler.TRH.Debug) Debug.Log("The split tag: " + split[0]);
            MethodInfo method;

            foreach (Type T in allTags) //check every single type to see if our method exists. probably suboptimal, but screw finding a better way.
            {
                method = T.GetMethod(split[0], BindingFlags.NonPublic | BindingFlags.Static);
                if (TagRoutineHandler.TRH.Debug) Debug.Log("This type (" + T.FullName + "), says method: " + method);
                if (method != null)
                {
                    while (splitNoFirst.Count < method.GetParameters().Length)
                    {
                        splitNoFirst.Add(""); //just add empty arguments to fill up. might cause problems if you don't set default checks
                    }
                    if (fire) method.Invoke(instance, splitNoFirst.ToArray()); //fire the method
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Background thread version of Parse.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>True if we found a valid (non-built-in, like italicize) tag, false otherwise.</returns>
        public static async Awaitable<bool> ParseAsync(string tag, bool fire = true)
        {
            await Awaitable.BackgroundThreadAsync();
            return Parse(tag, fire);
        }

        /// <summary>
        /// Removes any tag parser tags from a string. Ideally also fires those tag's affects, so that any expected behavior still occurs.
        /// </summary>
        /// <param name="sToClean">String you want scrubbed.</param>
        /// <param name="fire">Decides whether or not to fire the effects of any found valid tagparser tag.</param>
        /// <param name="includeBuiltInTagsInReturnedString">Decides whether or not to include any non-tagparser tags in the returned string. Ie. italics tag, etc. </param>>
        /// <returns>The string minus any TagParser valid tags.</returns>
        public static string CleanOutTags(string sToClean, bool fire = true, bool includeBuiltInTagsInReturnedString = true)
        {
            string sToReturn = "";
            string possibleTag;
            char[] chars = sToClean.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != '<') sToReturn += chars[i];
                else
                {
                    possibleTag = "";
                    while (chars[i] != '>') //probably dangerous if you just include a random ass '<'. careful!
                    {
                        if (i >= chars.Length)
                        {
                            Debug.LogWarning("The tag parser found a random '<' symbol, that never closes! " +
                                "Very scary. You're going to be getting an empty string for this!");
                            return "";
                        }
                        possibleTag += chars[i];
                        i++;
                    }

                    possibleTag += chars[i];
                    
                    if(!Parse(possibleTag, fire) && includeBuiltInTagsInReturnedString) sToReturn += possibleTag; 
                }
            }

            return sToReturn;
        }
        /// <summary>
        /// Background thread version of CleanOutTags
        /// </summary>
        /// <param name="sToClean">String you want scrubbed.</param>
        /// <param name="fire">Decides whether or not to fire the effects of any found valid tagparser tag.</param>
        /// <param name="includeBuiltInTagsInReturnedString">Decides whether or not to include any non-tagparser tags in the returned string. Ie. italics tag, etc. </param>>
        /// <returns>The string minus any TagParser valid tags.</returns>
        public static async Awaitable<string> CleanOutTagsAsync(string sToClean, bool fire = true, bool includeBuiltInTagsInReturnedString = true)
        {
            await Awaitable.BackgroundThreadAsync();
            string sToReturn = "";
            string possibleTag;
            char[] chars = sToClean.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != '<') sToReturn += chars[i];
                else
                {
                    possibleTag = "";
                    while (chars[i] != '>') //probably dangerous if you just include a random ass '<'. careful!
                    {
                        if (i >= chars.Length)
                        {
                            Debug.LogWarning("The tag parser found a random '<' symbol, that never closes! " +
                                "Very scary. You're going to be getting an empty string for this!");
                            return "";
                        }
                        possibleTag += chars[i];
                        i++;
                    }

                    possibleTag += chars[i];

                    if (!await ParseAsync(possibleTag, fire) && includeBuiltInTagsInReturnedString) sToReturn += possibleTag;
                }
            }

            return sToReturn;
        }
        /// <summary>
        /// Removes any built-in tags from a string, leaving in custom rich tags. By default, does not fire behavior of custom
        /// rich tags.
        /// </summary>
        /// <param name="sToClean">String you want scrubbed.</param>
        /// <param name="fireRichTags">Decides whether or not to fire the behavior of found custom rich tags.</param>
        /// <returns>Given string excluding built-in/non-functioning tags.</returns>
        public static string CleanOutBuiltInTags(string sToClean, bool fireRichTags = false)
        {
            string sToReturn = "";
            char[] chars = sToClean.ToCharArray();
            string possibleTag;
            for(int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != '<') sToReturn += chars[i];
                else
                {
                    possibleTag = "";
                    while (chars[i] != '>')
                    {
                        if(i >= chars.Length)
                        {
                            Debug.LogWarning("The tag parser found a random '<' symbol, that never closes! " +
                               "Very scary. You're going to be getting an empty string for this!");
                            return "";
                        }
                        possibleTag += chars[i];
                        i++;
                    }
                    possibleTag += chars[i];

                    if (Parse(possibleTag, fireRichTags)) sToReturn += possibleTag;
                }
            }

            return sToReturn;
        }
        /// <summary>
        /// Background thread version of CleanOutBuiltInTags
        /// </summary>
        /// <param name="sToClean">String you want scrubbed.</param>
        /// <param name="fireRichTags">Decides whether or not to fire the behavior of found custom rich tags.</param>
        /// <returns>Given string excluding built-in/non-functioning tags.</returns>
        public static async Awaitable<string> CleanOutBuiltInTagsAsync(string sToClean, bool fireRichTags = false)
        {
            await Awaitable.BackgroundThreadAsync();
            string sToReturn = "";
            char[] chars = sToClean.ToCharArray();
            string possibleTag;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != '<') sToReturn += chars[i];
                else
                {
                    possibleTag = "";
                    while (chars[i] != '>')
                    {
                        if (i >= chars.Length)
                        {
                            Debug.LogWarning("The tag parser found a random '<' symbol, that never closes! " +
                               "Very scary. You're going to be getting an empty string for this!");
                            return "";
                        }
                        possibleTag += chars[i];
                        i++;
                    }
                    possibleTag += chars[i];

                    if (await ParseAsync(possibleTag, fireRichTags)) sToReturn += possibleTag;
                }
            }

            return sToReturn;
        }

        public static int CleanedOutBuiltInTagsLength(string sToClean, bool fireRichTags = false)
        {
            return CleanOutBuiltInTags(sToClean, fireRichTags).Length;
        }

        public static async Awaitable<int> CleanedOutBuiltInTagsLengthAsync(string sToClean, bool fireRichTags = false)
        {
            string s = await CleanOutBuiltInTagsAsync(sToClean, fireRichTags);
            return s.Length;
        }
    }
}
