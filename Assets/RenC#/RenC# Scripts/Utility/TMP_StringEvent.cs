using UnityEngine;
using TMPro;
using RenCSharp.EXPERIMENTAL;
namespace RenCSharp
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMP_StringEvent : MonoBehaviour
    {
        [SerializeField] private string eventName, defaultText;
        private TextMeshProUGUI textbox;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            textbox = GetComponent<TextMeshProUGUI>();
            Event_Bus.AddStringEvent(eventName, SetString);
            SetString(defaultText);
        }

        // Update is called once per frame
        void OnDisable()
        {
            Event_Bus.TryRemoveStringEvent(eventName);
        }

        void SetString(string s)
        {
            textbox.text = s;
        }
    }
}
