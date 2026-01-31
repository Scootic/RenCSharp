using UnityEngine;
using TMPro;
using EXPERIMENTAL;
namespace RenCSharp
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMP_FloatEvent : MonoBehaviour
    {
        [SerializeField] private string prefixText, suffixText;
        [SerializeField] private string eventName;
        private TextMeshProUGUI text;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            if(text == null) text = GetComponent<TextMeshProUGUI>();
            Event_Bus.AddFloatEvent(eventName, UpdateString);
        }

        // Update is called once per frame
        void OnDisable()
        {
            Event_Bus.TryRemoveFloatEvent(eventName);
        }

        void UpdateString(float f)
        {
            text.text = prefixText + f.ToString("0.##") + suffixText;
        }
    }
}
