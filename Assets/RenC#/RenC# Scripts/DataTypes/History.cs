using System;

namespace RenCSharp
{
    /// <summary>
    /// Data type for handling histories. Should be saveable.
    /// </summary>
    [Serializable]
    public struct History 
    {
        public string[] SpeakerNames;
        public string[] DialogBoxes;
        public byte HistoryLength;

        public History(byte length) 
        {
            HistoryLength = length;
            SpeakerNames = new string[length];
            DialogBoxes = new string[length];
        }
    }
}
