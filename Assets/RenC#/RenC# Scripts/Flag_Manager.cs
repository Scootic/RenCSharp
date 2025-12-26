using System.Collections.Generic;
using UnityEngine;
namespace RenCSharp
{
    /// <summary>
    /// A static class for referencing and comparing flags. Stores flags in a dictionary (based on whether or not you want it to be persistent)
    /// that has a string key, and an integer value. If a flag is not in the dictionary, it's value will default to 0 when grabbed. If you
    /// want to do bitwise comparisons when using flags, I strongly recommend incrementing your flags, rather than manually setting them.
    /// </summary>
    public static class Flag_Manager
    {
        private static Dictionary<string, int> curFlags = new();
        private static Dictionary<string, int> persistentFlags = new();
        /// <summary>
        /// Should really only be used for saving, please don't mess with!
        /// </summary>
        public static Dictionary<string, int> GetSaveDataFlags => curFlags;
        /// <summary>
        /// Please don't touch this unless you know what you're doing fr fr
        /// </summary>
        public static Dictionary<string, int> GetPersistentDataFlags => persistentFlags;

        public static void SetFlag(string id, int val, bool persistent = false)
        {
            Debug.Log("Setting flag: " + id + ", to: " + val);
            if (!persistent)
            {
                if (curFlags.ContainsKey(id)) curFlags[id] = val;
                else curFlags.Add(id, val);
            }
            else
            {
                if (persistentFlags.ContainsKey(id)) persistentFlags[id] = val;
                else persistentFlags.Add(id, val);
            }
        }

        public static int GetFlag(string id, bool persistent = false)
        {
            int val = 0;

            if (!persistent)
            {
                if (curFlags.ContainsKey(id)) val = curFlags[id];
            }
            else
            {
                if(persistentFlags.ContainsKey(id)) val = persistentFlags[id];
            }

            return val;
        }

        public static void IncrementFlag(string id, int valToIncreaseBy, bool persistent = false)
        {
            Debug.Log("Incrementing flag: " + id + ", increasing by: " + valToIncreaseBy);
            if (!persistent)
            {
                if (curFlags.ContainsKey(id)) curFlags[id] += valToIncreaseBy;
                else curFlags.Add(id, valToIncreaseBy);
            }
            else
            {
                if (persistentFlags.ContainsKey(id)) persistentFlags[id] += valToIncreaseBy;
                else persistentFlags.Add(id, valToIncreaseBy);
            }
        }
        /// <summary>
        /// Takes a flag token, and fills in one of the dictionaries based on the flag token's data. 
        /// </summary>
        /// <param name="ft">The flag token being parsed.</param>
        /// <param name="persistent">Whether or not we should be loading into the curFlags dict, or the persistentFlags dict.</param>
        public static void ReceiveFlagToken(FlagToken ft, bool persistent = false)
        {
            Dictionary<string, int> t = new();
            if (!persistent)
            {
                if (ft.FlagValues.Count == 0) { curFlags = new(); return; }
                for (int i = 0; i < ft.FlagValues.Count; i++)
                {
                    t.Add(ft.FlagIDs[i], ft.FlagValues[i]);
                }
                curFlags = t;
            }
            else
            {
                if(ft.FlagValues.Count == 0) { persistentFlags = new(); return; }
                for (int i = 0; i < ft.FlagValues.Count; i++)
                {
                    t.Add(ft.FlagIDs[i], ft.FlagValues[i]);
                }
                persistentFlags = t;
            }
        }
    }
}
