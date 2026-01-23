using UnityEngine;
using UnityEngine.UI;

namespace RenCSharp
{
    public class Fullscreen_Toggle : MonoBehaviour
    {
        [SerializeField] private string playerPrefName = "Fullscreen";
        [SerializeField] private Toggle theToggler;

        private void OnEnable()
        {
            int i = PlayerPrefs.GetInt(playerPrefName);
            bool b;
            if (i == 0) b = true;
            else b = false;
            Screen.fullScreen = b;
            theToggler.isOn = b;
        }

        public void ToggleFS(bool b)
        {
            Screen.fullScreen = b;
            int i = b ? 0 : 1;
            PlayerPrefs.SetInt(playerPrefName, i);
        }
    }
}
