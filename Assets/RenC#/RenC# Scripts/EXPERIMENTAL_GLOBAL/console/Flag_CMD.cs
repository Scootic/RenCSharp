using System.Collections.Generic;

namespace RenCSharp.EXPERIMENTAL
{
    public class Flag_CMD : Base_CMD
    {
        protected static void GetFlag(string string_flagName)
        {
            try
            {
                int value = Flag_Manager.GetFlag(string_flagName);
                RenConsole.Log($"{string_flagName}'s value: {value}.", LogSeverity.LogPositive, false);
            }
            catch
            {
                RenConsole.Log($"Somehow failed to grab a per-save Flag. Not sure how to help with that :(", LogSeverity.LogError);
            }
        }

        protected static void SetFlag(string string_flagName, string int_newValue)
        {

            if(int.TryParse(int_newValue, out int i)) 
            { 
                Flag_Manager.SetFlag(string_flagName, i);
                RenConsole.Log($"Set {string_flagName} to now have the value: {i}.", LogSeverity.LogPositive, false);
            }
            else
            {
                RenConsole.Log("Failed to Set a per-save Flag. Make sure the first argument is the flag name, and the second argument is an integer value.", LogSeverity.LogError);
            }
        }

        protected static void GetAllFlags()
        {
            if (Flag_Manager.GetSaveDataFlags.Count == 0) { RenConsole.Log("There are no stored flags in the per-save dictionary.", LogSeverity.LogWarning, false); return; }

            string s = "";

            foreach (KeyValuePair<string, int> kvp in Flag_Manager.GetSaveDataFlags)
            {
                s += $"{kvp.Key}: {kvp.Value}, ";
            }

            RenConsole.Log(s, LogSeverity.LogPositive);
        }

        protected static void GetPersistentFlag(string string_flagName)
        {
            try
            {
                int value = Flag_Manager.GetFlag(string_flagName, true);
                RenConsole.Log($"PERSISTENT - {string_flagName}'s value: {value}.", LogSeverity.LogWarning, false);
            }
            catch
            {
                RenConsole.Log("Somehow failed to grab a Persistent Flag. Not sure how to help with that :(", LogSeverity.LogError);
            }
        }

        protected static void SetPersistentFlag(string string_flagName, string int_newValue)
        {
            if(int.TryParse(int_newValue, out int i)) 
            { 
                Flag_Manager.SetFlag(string_flagName, i, true);
                RenConsole.Log($"PERSISTENT - Set {string_flagName}to now have the value: {i}.", LogSeverity.LogWarning, false);
            }
            else
            {
                RenConsole.Log("Failed to Set a Persistent Flag. Make sure the first argument is the flag name, and the second argument is an integer value.", LogSeverity.LogError);
            }
        }

        protected static void GetAllPersistentFlags()
        {
            if (Flag_Manager.GetPersistentDataFlags.Count == 0) { RenConsole.Log("There are no stored flags in the persistent flag dictionary.", LogSeverity.LogWarning, false); return; }

            string s = "";

            foreach (KeyValuePair<string, int> kvp in Flag_Manager.GetPersistentDataFlags)
            {
                s += $"{kvp.Key}: {kvp.Value}, ";
            }

            RenConsole.Log(s, LogSeverity.LogPositive);
        }
    }
}
