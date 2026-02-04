using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using EXPERIMENTAL;
using TMPro;
using System.Collections.Generic;
namespace RenCSharp.Tags
{
    public class TagParser
    {
        private static Assembly tagAssembly = Assembly.GetAssembly(typeof(TagParser));
        private static Type[] allTags = tagAssembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(Base_Tag))).ToArray();
        private static TagParser instance = new TagParser();
        private static TextMeshProUGUI currentTextMesh;

        public static TextMeshProUGUI SetCurrentTextMesh { set { currentTextMesh = value; } }

        /// <summary>
        /// Removes any tag parser tags from a string. Ideally also fires those tag's affects, so that any expected behavior still occurs.
        /// </summary>
        /// <param name="sToClean">String you want scrubbed.</param>
        /// <param name="fire">Decides whether or not to fire the effects of any found valid tagparser tag.</param>
        /// <param name="includeBuiltInTags">Decides whether or not to include any non-tagparser tags in the returned string. Ie. italics tag, etc. </param>>
        /// <returns>The string minus any TagParser valid tags.</returns>
        public static string CleanOutTags(string sToClean, bool fire = true, bool includeBuiltInTags = true)
        {
            string sToReturn = "";

            char[] chars = sToClean.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != '<') sToReturn += chars[i];
                else
                {
                    string possibleTag = "";
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
                    
                    if(!Parse(possibleTag, fire) && includeBuiltInTags) sToReturn += possibleTag; 
                }
            }

            return sToReturn;
        }

        /// <summary>
        /// Only exists so that the stupid hidden tags don't interfere with tagparser chicanery!
        /// </summary>
        /// <param name="s">The string whose length we want trimmed.</param>
        /// <returns>The length of the string, minus any character associated with a built-in tag.</returns>
        public static int StringLengthExcludeBuiltinTags(string s)
        {
            string lengthToReturn = "";
            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != '<') lengthToReturn += chars[i];
                else
                {
                    string possibleTag = "";
                    while (chars[i] != '>')
                    {
                        if (i >= chars.Length)
                        {
                            Debug.LogWarning("The tag parser's string length check found a tag that never closes. Scary AF! You're getting a -1 for this.");
                            return -1;
                        }
                        possibleTag += chars[i];
                        i++;
                    }
                    possibleTag += chars[i];
                    if (Parse(possibleTag, false)) lengthToReturn += possibleTag;
                }
            }

            return lengthToReturn.Length;
        }

        public static int StringIndexExcludeBuiltinTags(string s, int index)
        {
            int indexToReturn = index;
            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '<')
                {
                    string possibleTag = "";
                    while (chars[i] != '>')
                    {
                        if (i >= chars.Length)
                        {
                            Debug.LogWarning("The tag parser's string index check found a tag that never closes. Scary AF! You're getting the og index for this.");
                            return index;
                        }
                        possibleTag += chars[i];
                        i++;
                    }
                    possibleTag += chars[i];
                    if (!Parse(possibleTag, false)) indexToReturn -= possibleTag.Length;
                }
            }

            if (indexToReturn < s.Length)
            {
                return indexToReturn;
            }

            Debug.LogWarning("Your new index would be smaller than 0 if passed through! Returning og index.");
            return index;
        }

        /// <summary>
        /// Removes any flag pattern ( [flagName] ) from a string, replacing all [flagName] instances with the number assigned to the flag instead.
        /// </summary>
        /// <param name="sToClean">String you want scrubbed.</param>
        /// <returns>The string with flag values inserted where they belong, lmao.</returns>
        public static string CleanOutFlags(string sToClean)
        {
            string sToReturn = "";
            char[] chars = sToClean.ToCharArray();
            for(int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != '[') sToReturn += chars[i];
                else
                {
                    string possibleFlag = "";
                    i++; //move on to not include the [ bracket.
                    while (chars[i] != ']')
                    {
                        if(i >= chars.Length)
                        {
                            Debug.LogWarning("The tag parser found a random '[' symbol, that never closes! " +
                                "Very scary. You're going to be getting an empty string for this!");
                            return "";
                        }
                        possibleFlag += chars[i];
                        i++;
                    }
                    string flagString = Flag_Manager.GetFlag(possibleFlag).ToString();
                    sToReturn += flagString;
                }
            }
            return sToReturn;
        }

        /// <summary>
        /// Find if we have ourselves a valid tag parser tag. Fire that mf's functionality if we do.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>True if we found a valid tag, false otherwise.</returns>
        public static bool Parse(string tag, bool fire = true)
        {
            Debug.Log("Length of valid tag types: " + allTags.Length);
            string[] split = Regex.Split(tag, "[=,]"); //0 should be function name, 1+ is arguments
            List<object> splitNoFirst = new List<object>();
            for (int i = 1; i < split.Length; i++)
            {
                splitNoFirst.Add(split[i]);
            }

            split[0] = Regex.Replace(split[0], "/", "End"); //swap out the slash symbol for "End" which is what it represents
            split[0] = Regex.Replace(split[0], "[<>]", ""); //get rid of tag wrapper symbols

            for(int i = 0; i < splitNoFirst.Count; i++)
            {
                splitNoFirst[i] = Regex.Replace((string) splitNoFirst[i], "[<>]", "");
            }

            if (!split[0].Contains("End")) splitNoFirst.Insert(0, currentTextMesh); //if it's an "End" function, we can assume no arguments are being passed in

            Debug.Log("The split tag: " + split[0]);
            MethodInfo method;

            foreach(Type T in allTags) //check every single type to see if our method exists. probably suboptimal, but screw finding a better way.
            {
                method = T.GetMethod(split[0], BindingFlags.NonPublic | BindingFlags.Static);
                Debug.Log("This type (" + T.FullName + "), says method: " + method);
                if(method != null) 
                {
                    if(fire) method.Invoke(instance, splitNoFirst.ToArray()); //invoke on the instance obj?
                    return true;
                }
            }

            return false;
        }
    }
}
