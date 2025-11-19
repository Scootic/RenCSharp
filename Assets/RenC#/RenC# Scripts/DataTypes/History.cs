using System;

namespace RenCSharp
{
    /// <summary>
    /// Data type for handling histories. Should be saveable.
    /// </summary>
    [Serializable]
    public class History 
    {
        public string[] SpeakerNames;
        public string[] DialogBoxes;

        public History(string[] speaker, string[] db) 
        {
            SpeakerNames = speaker;
            DialogBoxes = db;
        }
    }
}
