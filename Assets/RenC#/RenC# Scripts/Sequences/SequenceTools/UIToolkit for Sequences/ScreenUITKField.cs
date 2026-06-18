#if UNITY_EDITOR
using RenCSharp.Actors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RenCSharp.Sequences
{
    [UxmlElement]
    public partial class ScreenUITKField : BaseField<Screen>
    {
        private readonly VisualElement ContentElement;
        private readonly ObjectField ActorField;
        private readonly TextField DialogField;
        private readonly PropertyField ScreenEventsField; //really sad. Maybe a converter just to turn this boy to uxml?
        private readonly VisualElement GapSpace;

        public ScreenUITKField() : this(null) { }

        public ScreenUITKField(string labelText, Screen s) : base(labelText, new VisualElement())
        {
            ContentElement = this.Q<VisualElement>(className: inputUssClassName);
            ContentElement.style.flexDirection = FlexDirection.Column;
            AddToClassList(alignedFieldUssClassName);
            labelElement.style.marginBottom = 1;

            ActorField = new ObjectField();
            ActorField.label = "Speaker:";
            ActorField.objectType = typeof(Actor);
            ActorField.value = s.Speaker;
            ContentElement.Add(ActorField);

            DialogField = new TextField();
            DialogField.label = "Dialog:";
            DialogField.SetVerticalScrollerVisibility(ScrollerVisibility.Auto); //???
            DialogField.style.whiteSpace = WhiteSpace.Normal;
            DialogField.multiline = true;
            ContentElement.Add(DialogField);

            ScreenEventsField = new PropertyField();
            ScreenEventsField.label = "Screen Actions:";
            ContentElement.Add(ScreenEventsField);

            GapSpace = new VisualElement();
            GapSpace.style.height = 20;
            ContentElement.Add(GapSpace);
        }

        public ScreenUITKField(string labelText) : base(labelText, new VisualElement())
        {
            ContentElement = this.Q<VisualElement>(className: inputUssClassName);
            ContentElement.style.flexDirection = FlexDirection.Column;
            AddToClassList(alignedFieldUssClassName);
            labelElement.style.marginBottom = 1;

            ActorField = new ObjectField();
            ActorField.objectType = typeof(Actor);
            ActorField.label = "Speaker:";
            ActorField.value = null;
            ContentElement.Add(ActorField);

            DialogField = new TextField();
            DialogField.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
            DialogField.style.whiteSpace = WhiteSpace.Normal;
            DialogField.label = "Dialog:";
            DialogField.value = "";
            DialogField.multiline = true;
            ContentElement.Add(DialogField);

            ScreenEventsField = new PropertyField();
            ScreenEventsField.label = "Screen Actions:";
            ContentElement.Add(ScreenEventsField);

            GapSpace = new VisualElement();
            GapSpace.style.height = 20;
            ContentElement.Add(GapSpace);
        }

        public Screen SetValue
        {
            set
            {
                ActorField.value = value.Speaker;
                DialogField.value = value.Dialog;
            }
        }

        public Actor GetActor
        {
            get
            {
                return ActorField.value as Actor;
            }
        }

        public string GetDialog
        {
            get
            {
                return DialogField.value;
            }
        }

        public SerializedProperty SetScreenEventsProperty
        {
            set
            {
                if (value != null)
                {
                    ScreenEventsField.BindProperty(value);
                }
                else
                {
                    Debug.LogError("SetScreenEventsProperty was passed a null property!");
                }
            }
        }

        public void SetCustomLabel(VisualElement customLabel)
        {
            labelElement.Clear();
            labelElement.Add(customLabel);
        }

        public void SetCustomContent(VisualElement customContent)
        {
            ContentElement.Clear();
            ContentElement.Add(customContent);
        }
    }
}
#endif