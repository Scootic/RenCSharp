using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline
{
    [UxmlElement]
    public partial class ScalingPropertyField : PropertyField
    {
        protected ObjectField subField;
        protected Label subLabel;
        protected VisualElement dragDropBox;
        protected readonly float width;
        public ScalingPropertyField()
        {
            RegisterValueChangeCallback(ResizeElements);
        }

        public ScalingPropertyField(float w)
        {
            style.width = w;
            width = w;
            RegisterValueChangeCallback(ResizeElements);
        }

        protected void ResizeElements(SerializedPropertyChangeEvent evt)
        {

        }
    }
}
