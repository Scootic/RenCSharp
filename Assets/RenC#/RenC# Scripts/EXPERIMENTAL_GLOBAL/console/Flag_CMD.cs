namespace RenCSharp.EXPERIMENTAL
{
    public class Flag_CMD : Base_CMD
    {
        protected static void GetFlag(string flagName)
        {
            int value = Flag_Manager.GetFlag(flagName);
            RenConsole.Log($"{flagName}'s value: {value}.", LogSeverity.LogPositive, false);
        }

        protected static void SetFlag(string flagName, int newValue)
        {
            Flag_Manager.SetFlag(flagName, newValue);
            RenConsole.Log($"Set {flagName} to now have the value: {newValue}.", LogSeverity.LogPositive, false);
        }

        protected static void GetPersistentFlag(string flagName)
        {
            int value = Flag_Manager.GetFlag(flagName, true);
            RenConsole.Log($"PERSISTENT - {flagName}'s value: {value}.", LogSeverity.LogWarning, false);
        }

        protected static void SetPersistentFlag(string flagName, int newValue)
        {
            Flag_Manager.SetFlag(flagName, newValue, true);
            RenConsole.Log($"PERSISTENT - Set {flagName} to now have the value: {newValue}.", LogSeverity.LogWarning, false);
        }
    }
}
