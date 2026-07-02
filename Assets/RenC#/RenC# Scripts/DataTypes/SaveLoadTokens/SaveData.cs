using System;
namespace RenCSharp
{
    /// <summary>
    /// The SaveData class that stores all the information that a player would want to preserve. While you can edit this to suit your needs,
    /// it might make existing files obsolete or corrupt, especially if you're altering logic that ties into pre-existing systems. Alter at
    /// your discretion. (I recommend using try-catches whenever you add new elements as a way to provide fail-safes for old save data files.)
    /// </summary>
    [Serializable]
    public struct SaveData 
    {
        /// <summary>
        /// The current screen index of the current sequence, so that the Script_Manager knows what screen the player is on.
        /// </summary>
        public int CurrentScreenIndex;
        /// <summary>
        /// All information of the current state of the screen, like actors that are on-screen, particles, and audio that's playing.
        /// </summary>
        public ScreenToken ScreenInformation;
        public string CurrentSequenceAsset, FileName;
        /// <summary>
        /// current stored textbox history
        /// </summary>
        public History CurrentHistory;
        /// <summary>
        /// All the save-specific flags; keys and values.
        /// </summary>
        public FlagToken CurrentFlags;
        /// <summary>
        /// Keys for ReplaceText dict
        /// </summary>
        public string[] ReplacedTexts;
        /// <summary>
        /// Values for ReplaceText dict
        /// </summary>
        public string [] ReplacingTexts;
        /// <summary>
        /// Raw byte value of the .png of screenshot when saving
        /// </summary>
        public byte[] SaveScreenshot;
    }
}
