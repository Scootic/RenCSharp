#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
namespace RenCSharp.EXPERIMENTAL
{
    [UxmlElement]
    public partial class RenConsole_TextField : TextField
    {
        private readonly VisualElement backgroundVE;
        private readonly TextField inputField;

        public RenConsole_TextField() : this(null) { }

        public RenConsole_TextField(string labelText)
        {
            backgroundVE = this.Q<VisualElement>();
            backgroundVE.style.flexDirection = FlexDirection.Row;
            AddToClassList(alignedFieldUssClassName);
            backgroundVE.style.marginBottom = 0;
            backgroundVE.style.marginRight = 0;
            backgroundVE.style.marginLeft = 0;
            backgroundVE.style.marginTop = 0;

            inputField = backgroundVE.Q<TextField>();
            inputField.AddToClassList(inputUssClassName);
            inputField.Q<VisualElement>().style.color = Color.white;
            inputField.Q<VisualElement>().style.backgroundColor = CoolColors._50PercentBlack;
        }
    }
}
#endif