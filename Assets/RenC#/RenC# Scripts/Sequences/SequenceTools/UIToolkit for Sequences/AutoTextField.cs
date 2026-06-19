#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.UIElements;
namespace RenCSharp.Sequences
{
    [UxmlElement]
    public partial class AutoTextField : BaseField<string>
    {
        private static List<string> cacheList;
        private readonly VisualElement ContentElement;
        private readonly TextField inputField;
        public TextField GetInputField { get { return inputField; } }
        private readonly DropdownField dropdownField;
        public DropdownField GetDropdownField { get { return dropdownField; } }
        private readonly List<string> validAutoText;

        private const string inputFieldClassName = "autotextfield__input-text";
        private const string dropdownFieldClassName = "autotextfield__input-dropdown";

        public AutoTextField() : this(null) { }

        public AutoTextField(string labelText) : base(labelText, new VisualElement())
        {
            ContentElement = this.Q<VisualElement>(className: inputUssClassName);
            ContentElement.style.flexDirection = FlexDirection.Row;
            AddToClassList(alignedFieldUssClassName);

            inputField = new();
            inputField.AddToClassList(inputFieldClassName);
            ContentElement.Add(inputField);
            inputField.style.minWidth = 100f;
            inputField.RegisterValueChangedCallback(evt => OnKeyInput(evt.newValue));

            dropdownField = new();
            dropdownField.AddToClassList(dropdownFieldClassName);
            dropdownField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                dropdownField.value = evt.newValue;
                inputField.value = evt.newValue;
                value = evt.newValue;
            });
            ContentElement.Add(dropdownField);
            dropdownField.style.minWidth = 100f;
        }

        public AutoTextField(string labelText, List<string>autoText) : base(labelText, new VisualElement())
        {
            validAutoText = autoText;

            AddToClassList(alignedFieldUssClassName);

            ContentElement = this.Q<VisualElement>(className: inputUssClassName);
            ContentElement.style.flexDirection = FlexDirection.Row;

            inputField = new();
            inputField.AddToClassList(inputFieldClassName);
            ContentElement.Add(inputField);
            inputField.style.minWidth = 100f;
            inputField.RegisterValueChangedCallback(evt => OnKeyInput(evt.newValue));

            dropdownField = new();
            dropdownField.AddToClassList(dropdownFieldClassName);
            dropdownField.RegisterCallback<ChangeEvent<string>>(evt => 
            {
                dropdownField.value = evt.newValue;
                inputField.value = evt.newValue;
                value = evt.newValue;
            });
            ContentElement.Add(dropdownField);
            dropdownField.style.minWidth = 100f;
        }

        public void OnKeyInput(string newVal)
        {
            inputField.value = newVal;
            dropdownField.value = newVal;
            value = newVal;
            if (validAutoText == null) return;

            List<string> copyOfSource = new List<string>(new HashSet<string>(validAutoText));
            cacheList = new List<string>(); //max shown is 7?
            dropdownField.choices = cacheList;
            int validAutoTextLength = copyOfSource.Count;

            if (newVal.Length >= 3) //require a string that's at least three letters long before we care.
            {
                for (int i = 0; i < validAutoTextLength && i < copyOfSource.Count; i++)
                {
                    if (cacheList.Count >= 7) break;
                    if (copyOfSource[i].ToLower().Contains(newVal.ToLower())) //if it includes
                    {
                        cacheList.Add(copyOfSource[i]);
                        copyOfSource.RemoveAt(i);
                        validAutoTextLength--;
                        i--;
                    }
                }

                if (cacheList.Count < 7) //if the includes list doesn't fill up the max amount
                {
                    string keywords = inputField.value.ToLower();
                    for (int i = 0; i < validAutoTextLength && i < cacheList.Count; i++)
                    {
                        if (cacheList.Count >= 7) break;
                        int distance = StringExtend.LevenshteinDistance(copyOfSource[i], keywords, false);
                        bool closeEnough = (copyOfSource.Count * 0.5f) > distance;
                        if (closeEnough)
                        {
                            cacheList.Add(copyOfSource[i]);
                            copyOfSource.RemoveAt(i);
                            validAutoTextLength--;
                            i--;
                        }
                    }

                    for (int i = 0; i < validAutoTextLength && i < copyOfSource.Count; i++)
                    {
                        if (cacheList.Count >= 7) break;
                        if (copyOfSource[i].ToLower().StartsWith(newVal.ToLower()[0])) //if it starts with the same letter
                        {
                            cacheList.Add(copyOfSource[i]);
                            copyOfSource.RemoveAt(i);
                            validAutoTextLength--;
                            i--;
                        }
                    }
                }
            }

            bool prevVis = dropdownField.visible;
            dropdownField.visible = cacheList.Count > 2 && newVal != "";
            dropdownField.enabledSelf = dropdownField.visible;
            if(prevVis != dropdownField.visible) ContentElement.MarkDirtyRepaint(); //repaint if the visibility changed.
            //cacheList.Reverse(); Reverse based on the position of the expanded menu to the button?
            dropdownField.choices = cacheList;
        }
    }
}
#endif