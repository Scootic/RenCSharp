using UnityEngine;
using EXPERIMENTAL;
using System.Collections;
namespace RenCSharp
{
    /// <summary>
    /// Simple GO to hang around until we make sure a string has been passed through
    /// to an event. Horrific and dangerous, please use sparingly.
    /// </summary>
    public class String_Data_From_Menu : MonoBehaviour
    {
        [SerializeField] private string eventName;
        [SerializeField] private string defaultString;
        [SerializeField, Tooltip("How long the GO checks for its event before self-deletion.")] private float activeDuration = 0.5f;
        private string passValue;
        void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
            if (passValue == null) passValue = defaultString;
            StartCoroutine(CheckForEvent());
        }

        public void SetPassValue(string s)
        {
            passValue = s;
        }

        // Update is called once per frame
        void Update()
        {
            if(Event_Bus.TryFireStringEvent(eventName, passValue)) Destroy(gameObject);
        }

        IEnumerator CheckForEvent()
        {
            float t = 0;
            while (t < activeDuration)
            {
                t += Time.deltaTime;
                if (Event_Bus.TryFireStringEvent(eventName, passValue)) Destroy(gameObject);
                yield return null;
            }
            //we've managed to be alive for activeDuration, that means our event doesn't exist and probably won't ever. die!
            Destroy(gameObject);
        }
    }
}
