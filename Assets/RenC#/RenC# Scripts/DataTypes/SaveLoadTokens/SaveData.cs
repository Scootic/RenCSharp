
using System;
namespace RenCSharp
{
    /// <summary>
    /// The SaveData class that stores all the information that a player would want to preserve. While you can edit this to suit your needs,
    /// it might make existing files obsolete or corrupt, especially if you're altering logic that ties into pre-existing systems. Alter at
    /// your discretion.
    /// </summary>
    [Serializable]
    public struct SaveData 
    {
        public int CurrentScreenIndex;
        public ScreenToken ScreenInformation;
        public string CurrentSequenceAsset, PlayerName, FileName;
        public History CurrentHistory;
        public FlagToken CurrentFlags;
        public byte[] SaveScreenshot;
    }
}
