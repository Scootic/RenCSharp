using UnityEngine;
using UnityEngine.UI;

namespace EXPERIMENTAL
{
    public class PlayerPref_Slider : MonoBehaviour
    {
        [SerializeField] private string playerPrefRef;
        [SerializeField] private string UpdateReferenceActionName;

        private void OnEnable()
        {
            GetComponent<Slider>().value = PlayerPrefs.GetFloat(playerPrefRef);
        }

        public void UpdatePref(float f)
        {
            PlayerPrefs.SetFloat(playerPrefRef, f);
            Event_Bus.TryFireFloatEvent(UpdateReferenceActionName, f);
        }
    }
}
