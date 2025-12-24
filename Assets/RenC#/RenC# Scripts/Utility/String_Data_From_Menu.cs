using UnityEngine;
using EXPERIMENTAL;
namespace RenCSharp
{
    /// <summary>
    /// Simple GO to hang around until we make sure a string has been passed through
    /// to an event. Horrific and dangerous, please use sparingly.
    /// </summary>
    public class String_Data_From_Menu : MonoBehaviour
    {
        [SerializeField] private string eventName;
        private string passValue;
        void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
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
    }
}
