using UnityEngine;

namespace RenCSharp.EXPERIMENTAL
{
    /// <summary>
    /// A list of cool colors to be used by scripts as desired.
    /// </summary>
    public readonly struct CoolColors
    {
        #region Transparent Colors
        public static readonly Color transparent = new(1, 1, 1, 0);
        public static readonly Color slightTransWhiteGUI = new(1, 1, 1, 0.3f);
        public static readonly Color _75PercentBlack = new(0, 0, 0, 0.75f);
        public static readonly Color _50PercentBlack = new(0, 0, 0, 0.5f);

        public static readonly Color slightTransRedGUI = new(0.5f, 0, 0, 0.5f);
        public static readonly Color slightTransYellowGUI = new(0.5f, 0.45f, 0, 0.5f);
        public static readonly Color slightTransBlueGUI = new(0, 0.6f, 0.6f, 0.5f);
        public static readonly Color slightTransGrayGUI = new(0.5f, 0.5f, 0.5f, 0.5f);
        #endregion
        public static readonly Color selectedOliveColor = new(0.75f, 0.9f, 0.4f, 1);
        public static readonly Color undertaleOrange = new(1, 0.6075f, 0.2039f, 1);
        public static readonly Color undertaleBlue = new(0.2392f, 0.9215f, 1, 1);
    }
}
