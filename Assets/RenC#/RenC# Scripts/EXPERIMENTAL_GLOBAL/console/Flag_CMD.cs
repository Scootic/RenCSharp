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
    }
}
