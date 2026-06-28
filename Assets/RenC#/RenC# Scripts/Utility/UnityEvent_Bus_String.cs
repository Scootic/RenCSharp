using RenCSharp.EXPERIMENTAL;
using UnityEngine;
using UnityEngine.Events;

namespace RenCSharp
{
    public class UnityEvent_Bus_String : MonoBehaviour
    {
        [SerializeField] private UnityEvent<string> fellaToFire;
        [SerializeField] private string eventName;

        private void OnEnable()
        {
            Event_Bus.AddStringEvent(eventName, InvokeEvent);
        }

        private void OnDisable()
        {
            Event_Bus.TryRemoveStringEvent(eventName);
        }

        void InvokeEvent(string s)
        {
            fellaToFire?.Invoke(s);
        }
    }
}
