#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
namespace RenCSharp.EXPERIMENTAL
{
    [UxmlElement]
    public partial class RenConsole_InputField : TextField
    {
        private readonly TextField inputField;
        private readonly VisualElement inputBox;
        private readonly TextElement textBox; //I'm assuming default/placeholder text?

        public RenConsole_InputField() : this(null) { }

        public RenConsole_InputField(string labelText)
        {
            inputField = this.Q<TextField>();
            inputField.style.flexDirection = FlexDirection.Row;
            inputField.AddToClassList("renCSharp-console-input");
            AddToClassList(inputUssClassName);
            inputField.style.position = new StyleEnum<Position> { value = Position.Absolute };
            inputField.style.bottom = 0;
            inputField.style.color = Color.white;
            inputField.style.marginBottom = 0;
            inputField.style.marginRight = 0;
            inputField.style.marginLeft = 0;
            inputField.style.marginTop = 0;
            inputField.style.width = 1920;
            inputField.style.height = 40;

            inputBox = inputField.Q<VisualElement>("unity-text-input");
            inputBox.AddToClassList(alignedFieldUssClassName);
            inputBox.style.backgroundColor = CoolColors._50PercentBlack;
            inputBox.style.borderTopColor = CoolColors.transparent;
            inputBox.style.borderBottomColor = CoolColors.transparent;
            inputBox.style.borderLeftColor = CoolColors.transparent;
            inputBox.style.borderRightColor = CoolColors.transparent;

            textBox = inputBox.Q<TextElement>();
            if (textBox != null) 
            { 
                textBox.style.color = Color.white; 
                textBox.text = "Enter commands here. Type 'Help' for full list of commands..."; 
            }
        }
    }
}
#endif