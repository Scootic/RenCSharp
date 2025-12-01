
using System;

namespace RenCSharp
{
    [Serializable]
    public struct SaveData 
    {
        public int CurrentScreenIndex;
        public ScreenToken ScreenInformation;
        public string CurrentSequenceAsset, PlayerName;
        public SettingsToken CurrentSettings;
        public History CurrentHistory;
        public FlagToken CurrentFlags;
    }
}
