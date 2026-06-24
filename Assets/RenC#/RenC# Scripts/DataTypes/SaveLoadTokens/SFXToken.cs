using System;
namespace RenCSharp
{

    /// <summary>
    /// only use for super loopers!
    /// </summary>
    [Serializable]
    public struct SFXToken
    {
        public string SFXAddress;
        public float xPos, yPos, zPos;
        public float localVolume;
    }
}
