using System;
namespace RenCSharp
{

    /// <summary>
    /// only use for super loopers!
    /// </summary>
    [Serializable]
    public struct SFXToken
    {
        public string SFXAddress, SFXName; //dunno if even actually used sfxname for anything :)
        public float xPos, yPos, zPos;
        public float localVolume;
    }
}
