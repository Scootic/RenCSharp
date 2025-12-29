using EXPERIMENTAL;
using UnityEngine;
using UnityEngine.UI;
namespace RenCSharp
{
    [RequireComponent(typeof(Image))]
    public class ImageFill_FloatEvent : MonoBehaviour
    {
        [SerializeField] private string eventName;
        private Image me;

        void UpdateFill(float f)
        {
            me.fillAmount = f;
        }

        private void OnEnable()
        {
            if (me == null)
            {
                me = GetComponent<Image>();
                me.type = Image.Type.Filled;
            }
            Event_Bus.AddFloatEvent(eventName, UpdateFill);
        }

        // Update is called once per frame
        void OnDisable()
        {
            Event_Bus.TryRemoveFloatEvent(eventName);
        }
    }
}
