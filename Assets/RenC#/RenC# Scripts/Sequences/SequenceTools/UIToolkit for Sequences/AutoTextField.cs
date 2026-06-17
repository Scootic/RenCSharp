#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
namespace RenCSharp.Sequences
{
    [UxmlElement]
    public class AutoTextField : BaseField<string>
    {
        private readonly VisualElement ContentElement;
        private readonly TextField inputField;
        private readonly DropdownField dropdownField;
        private readonly List<string> validAutoText;

        public AutoTextField(string labelText, string[] autoText) : base(labelText, new VisualElement())
        {
            validAutoText = autoText.ToList();

            AddToClassList(ussClassName);

            ContentElement = this.Q<VisualElement>(className: inputUssClassName);

            inputField = new();
            inputField.AddToClassList("autotextfield__input-text");
            ContentElement.Add(inputField);

            dropdownField = new();
            dropdownField.AddToClassList("autotextfield__input-dropdown");
            dropdownField.RegisterCallback<ChangeEvent<string>>(evt => 
            {
                dropdownField.value = evt.newValue;
                inputField.value = evt.newValue;
            });
            ContentElement.Add(dropdownField);

            RegisterCallback<KeyDownEvent>(evt => OnKeyInput(evt));
        }

        private static void OnKeyInput(KeyDownEvent evt)
        {
            AutoTextField container = evt.currentTarget as AutoTextField;
            TextField inputField = container.inputField;

            if (evt.keyCode == KeyCode.Backspace)
            {
                inputField.value.Remove(inputField.value.Length - 1);
            }
            else if(evt.keyCode == KeyCode.Escape || evt.keyCode == KeyCode.Return)
            {
                evt.StopPropagation();
                return;
            }
            else
            {
                inputField.value += evt.character;
            }

            if (inputField.value.Length <= 0) return;

            DropdownField dropdownField = container.dropdownField;

            List<string> copyOfSource = new List<string>(new HashSet<string>(container.validAutoText)); 
            EditorExtend.EditorAutoCompleteParams.CacheCheckList = new List<string>(10); //max shown is 10
            int validAutoTextLength = copyOfSource.Count;

            for (int i = 0; i < validAutoTextLength && i < EditorExtend.EditorAutoCompleteParams.CacheCheckList.Count; i++)
            {
                if (copyOfSource[i].ToLower().StartsWith(evt.character.ToString().ToLower()))
                {
                    EditorExtend.EditorAutoCompleteParams.CacheCheckList.Add(container.validAutoText[i]);
                    copyOfSource.RemoveAt(i);
                    validAutoTextLength--;
                    i--;
                }
            }
            if (EditorExtend.EditorAutoCompleteParams.CacheCheckList.Count == 0) //do it again?
            {
                for (int i = 0; i < validAutoTextLength && i < EditorExtend.EditorAutoCompleteParams.CacheCheckList.Count; i++)
                {
                    if (copyOfSource[i].ToLower().StartsWith(evt.character.ToString().ToLower()))
                    {
                        EditorExtend.EditorAutoCompleteParams.CacheCheckList.Add(container.validAutoText[i]);
                        copyOfSource.RemoveAt(i);
                        validAutoTextLength--;
                        i--;
                    }
                }
            }
            if (EditorExtend.EditorAutoCompleteParams.CacheCheckList.Count < 10)
            {
                string keywords = inputField.value.ToLower();
                for (int i = 0; i < validAutoTextLength && i < EditorExtend.EditorAutoCompleteParams.CacheCheckList.Count; i++)
                {
                    int distance = StringExtend.LevenshteinDistance(copyOfSource[i], keywords, false);
                    bool closeEnough = (int)(copyOfSource.Count * 0.5f) > distance;
                    if (closeEnough)
                    {
                        EditorExtend.EditorAutoCompleteParams.CacheCheckList.Add(container.validAutoText[i]);
                        copyOfSource.RemoveAt(i);
                        validAutoTextLength--;
                        i--;
                    }
                }
            }

            dropdownField.choices = EditorExtend.EditorAutoCompleteParams.CacheCheckList;
        }
    }
}
#endif