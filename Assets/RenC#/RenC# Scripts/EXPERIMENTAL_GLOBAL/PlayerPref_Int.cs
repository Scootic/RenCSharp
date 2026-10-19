using UnityEngine;
using UnityEngine.UI;

namespace RenCSharp.EXPERIMENTAL
{
    /// <summary>
    /// Used to set a player prefence using a slider component. Useful for a settings menu where you can
    /// change how loud BGM can play, or how fast text should scroll.
    /// </summary>
    public sealed class PlayerPref_Int : MonoBehaviour
    {
        [SerializeField] private string playerPrefRef;
        [SerializeField] private string UpdateReferenceActionName;
        [SerializeField, Tooltip("0 and 1 if you want to use as a bool.")] private int defaultValue;

        private void OnEnable()
        {
            //GetComponent<Slider>().value = PlayerPrefs.GetFloat(playerPrefRef, defaultValue);
            if(TryGetComponent(out Toggle t))
            {
                t.isOn = PlayerPrefs.GetInt(playerPrefRef, defaultValue) == 1;
            }
        }

        public void UpdatePref(int i)
        {
            PlayerPrefs.SetInt(playerPrefRef, i);
            Event_Bus.TryFireIntEvent(UpdateReferenceActionName, i); //tries to fire a relevant event to immediately change a value to affect gameplay
        }

        public void UpdateBool(bool b)
        {
            if (b) PlayerPrefs.SetInt(playerPrefRef, 1);
            else PlayerPrefs.SetInt(playerPrefRef, 0);
            Event_Bus.TryFireBoolEvent(UpdateReferenceActionName, b);
        }
    }
}
