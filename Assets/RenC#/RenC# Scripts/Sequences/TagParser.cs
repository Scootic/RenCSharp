using System.Text.RegularExpressions;
using System.Reflection;
using System;
using UnityEngine;
namespace RenCSharp.Sequences
{
    public class TagParser
    {
        private static Type tp = typeof(TagParser);
        private static TagParser instance = new TagParser();
        public static bool Parse(string tag)
        {
            string[] split = Regex.Split(tag, "[=,]"); //0 should be function name, 1+ is arguments
            string[] splitNoFirst = new string[split.Length - 1];
            for (int i = 1; i < split.Length; i++)
            {
                splitNoFirst[i - 1] = split[i];
            }

            split[0] = Regex.Replace(split[0], "/", "End");
            split[0] = Regex.Replace(split[0], "[<>]", "");

            for(int i = 0; i < splitNoFirst.Length; i++)
            {
                splitNoFirst[i] = Regex.Replace(splitNoFirst[i], "[<>]", "");
            }
            
            Debug.Log("The split tag: " + split[0]);
            MethodInfo method = tp.GetMethod(split[0], BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Log(method);
            if (method != null) { method.Invoke(instance, splitNoFirst); return true; }
            else { return false; }
        }

        protected static void Speed(string value)
        {
            if (float.TryParse(value, out float valley))
            {
                valley = 1 / (valley * 10);
                Script_Manager.SM.SetSpeed(valley);
            }
        }

        protected static void EndSpeed()
        {
            Script_Manager.SM.SetSpeed(0, true);
        }
    }
}
