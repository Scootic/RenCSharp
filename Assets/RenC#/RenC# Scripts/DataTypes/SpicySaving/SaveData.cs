
using System;
namespace RenCSharp
{
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
