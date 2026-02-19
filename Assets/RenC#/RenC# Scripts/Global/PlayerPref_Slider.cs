using UnityEngine;
using UnityEngine.UI;

namespace EXPERIMENTAL
{
    /// <summary>
    /// Used to set a player prefence using a slider component. Useful for a settings menu where you can
    /// change how loud BGM can play, or how fast text should scroll.
    /// </summary>
    public sealed class PlayerPref_Slider : MonoBehaviour
    {
        [SerializeField] private string playerPrefRef;
        [SerializeField] private string UpdateReferenceActionName;
        [SerializeField] private float defaultValue;

        private void OnEnable()
        {
            GetComponent<Slider>().value = PlayerPrefs.GetFloat(playerPrefRef, defaultValue);
        }

        public void UpdatePref(float f)
        {
            PlayerPrefs.SetFloat(playerPrefRef, f);
            Event_Bus.TryFireFloatEvent(UpdateReferenceActionName, f);
        }
    }
}
