namespace RenCSharp.EXPERIMENTAL
{
    public class Cheat_CMD : Base_CMD
    {
        /// <summary>
        /// Sets a flag to be used by somebody to check if player is invincible. Whatever that means for your project.
        /// </summary>
        /// <param name="boolean_On">Decides to toggle god mode on or off, on if true.</param>
        protected static void TGM(string boolean_On)
        {
            try
            {
                bool.TryParse(boolean_On, out bool b);
                if (b)
                {
                    Flag_Manager.SetFlag("tgm", 1);
                    RenConsole.Log("God Mode Enabled", LogSeverity.LogPositive, false);
                }
                else
                {
                    Flag_Manager.SetFlag("tgm", 0);
                    RenConsole.Log("God Mode Disabled", LogSeverity.LogPositive, false);
                }
            }
            catch
            {
                RenConsole.Log("Couldn't fire TGM. Make sure you give a boolean argument.", LogSeverity.LogError);
            }
        }
    }
}
