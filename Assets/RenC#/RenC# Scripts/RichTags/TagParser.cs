using EXPERIMENTAL;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
namespace RenCSharp.Tags
{
    public class TagParser
    {
        private static Assembly tagAssembly = Assembly.GetAssembly(typeof(TagParser));
        private static Type[] allTags = tagAssembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(Base_Tag))).ToArray();
        private static TagParser instance = new TagParser();

        /// <summary>
        /// Removes any tag parser tags from a string. Ideally also fires those tag's affects, so that any expected behavior still occurs.
        /// </summary>
        /// <param name="sToClean">String you want scrubbed.</param>
        /// <returns>The string minus any TagParser valid tags.</returns>
        public static string CleanOutTags(string sToClean) 
        {
            string sToReturn = "";

            char[] chars = sToClean.ToCharArray();
            for(int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != '<') sToReturn += chars[i];
                else
                {
                    string possibleTag = "";
                    while (chars[i] != '>') //probably dangerous if you just include a random ass '<'. careful!
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
                    if (!Parse(possibleTag)) sToReturn += possibleTag;
                }
            }

            return sToReturn;
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
        public static bool Parse(string tag)
        {
            Debug.Log("Length of valid tag types: " + allTags.Length);
            string[] split = Regex.Split(tag, "[=,]"); //0 should be function name, 1+ is arguments
            string[] splitNoFirst = new string[split.Length - 1];
            for (int i = 1; i < split.Length; i++)
            {
                splitNoFirst[i - 1] = split[i];
            }

            split[0] = Regex.Replace(split[0], "/", "End"); //swap out the slash symbol for "End" which is what it represents
            split[0] = Regex.Replace(split[0], "[<>]", ""); //get rid of tag wrapper symbols

            for(int i = 0; i < splitNoFirst.Length; i++)
            {
                splitNoFirst[i] = Regex.Replace(splitNoFirst[i], "[<>]", "");
            }
            
            Debug.Log("The split tag: " + split[0]);
            MethodInfo method;

            foreach(Type T in allTags) //check every single type to see if our method exists. probably suboptimal, but screw finding a better way.
            {
                method = T.GetMethod(split[0], BindingFlags.NonPublic | BindingFlags.Static);
                Debug.Log("This type (" + T.FullName + "), says method: " + method);
                if(method != null) 
                {
                    method.Invoke(instance, splitNoFirst); //invoke on the instance obj?
                    return true;
                }
            }

            return false;
        }
    }
}
