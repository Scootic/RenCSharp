#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System.Collections.Generic;
namespace RenCSharp.Sequences
{
    public class PlayerChoiceUITKField : BaseField<Player_Choice>
    {
        private readonly VisualElement ContentElement;
        private readonly TextField ChoiceTextField;
        private readonly ObjectField ResultingSequenceField;
        private readonly PropertyField ConditionsField;

        public PlayerChoiceUITKField(string labelText) : base(labelText, new VisualElement())
        {
            ContentElement = this.Q<VisualElement>(className: inputUssClassName);
            AddToClassList(alignedFieldUssClassName);
            labelElement.style.marginBottom = 1;

            ChoiceTextField = new TextField("Choice Text:");

            ContentElement.Add(ChoiceTextField);

            ResultingSequenceField = new ObjectField("Resulting Sequence:");
            ResultingSequenceField.objectType = typeof(Sequence);

            ConditionsField = new PropertyField();

            ContentElement.Add(ResultingSequenceField);
            ContentElement.Add(ConditionsField);
        }

        public Player_Choice SetPlayerChoice //?
        {
            set
            {
                if (ContentElement == null) return;
                ResultingSequenceField.value = value.ResultingSequence;
                ChoiceTextField.value = value.ChoiceText;
            }
        }

        public SerializedProperty SetConditionsProperty 
        { 
            set
            {
                ConditionsField.BindProperty(value);
            } 
        }

        public string GetChoiceText
        {
            get
            {
                return ChoiceTextField.value;
            }
        }

        public Sequence GetResultingSequence
        {
            get
            {
                return ResultingSequenceField.value as Sequence;
            }
        }
    }
}
#endif