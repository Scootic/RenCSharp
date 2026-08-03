#if UNITY_EDITOR
using RenCSharp.Actors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using RenCSharp.EXPERIMENTAL;
namespace RenCSharp.Sequences
{
    [UxmlElement]
    public partial class ScreenUITKField : BaseField<Screen>
    {
        private readonly VisualElement ContentElement;
        private readonly ObjectField ActorField;
        private readonly TextField DialogField;
        private readonly PropertyField ScreenEventsField; //really sad. Maybe a converter just to turn this boy to uxml?
        private readonly VisualElement StartGapSpace, EndGapSpace;

        public ScreenUITKField() : this(null) { }

        public ScreenUITKField(string labelText, Screen s) : base(labelText, new VisualElement())
        {
            ContentElement = this.Q<VisualElement>(className: inputUssClassName);
            ContentElement.style.flexDirection = FlexDirection.Column;
            ContentElement.style.backgroundColor = CoolColors.slightTransWhiteGUI;
            ContentElement.style.color = CoolColors.slightTransWhiteGUI;
            AddToClassList(alignedFieldUssClassName);
            labelElement.style.marginBottom = 1;

            StartGapSpace = new();
            StartGapSpace.style.height = 10;
            StartGapSpace.style.color = CoolColors._75PercentBlack;
            StartGapSpace.style.backgroundColor = CoolColors._75PercentBlack;
            ContentElement.Add(StartGapSpace);

            ActorField = new();
            ActorField.label = "Speaker:";
            ActorField.objectType = typeof(Actor);
            ActorField.value = s.Speaker;
            ActorField.Q<Label>().style.color = Color.white;
            ContentElement.Add(ActorField);

            DialogField = new();
            DialogField.label = "Dialog:";
            DialogField.Q<Label>().style.color = Color.white;
            DialogField.verticalScrollerVisibility = ScrollerVisibility.Auto; //???
            DialogField.style.whiteSpace = WhiteSpace.Normal;
            DialogField.multiline = true;
            DialogField.style.color = Color.white;
            ContentElement.Add(DialogField);

            ScreenEventsField = new();
            ScreenEventsField.label = "Screen Actions:";
            ScreenEventsField.style.color = Color.white;
            ScreenEventsField.pickingMode = PickingMode.Ignore;
            ScreenEventsField.focusable = false;
            ContentElement.Add(ScreenEventsField);

            EndGapSpace = new();
            EndGapSpace.style.height = 20;
            ContentElement.Add(EndGapSpace);
        }

        public ScreenUITKField(string labelText) : base(labelText, new VisualElement())
        {
            ContentElement = this.Q<VisualElement>(className: inputUssClassName);
            ContentElement.style.flexDirection = FlexDirection.Column;
            ContentElement.style.backgroundColor = CoolColors.slightTransWhiteGUI;
            ContentElement.style.color = CoolColors.slightTransWhiteGUI;
            AddToClassList(alignedFieldUssClassName);
            labelElement.style.marginBottom = 1;

            StartGapSpace = new();
            StartGapSpace.style.height = 10;
            StartGapSpace.style.color = CoolColors._75PercentBlack;
            StartGapSpace.style.backgroundColor = CoolColors._75PercentBlack;
            ContentElement.Add(StartGapSpace);

            ActorField = new();
            ActorField.objectType = typeof(Actor);
            ActorField.label = "Speaker:";
            ActorField.Q<Label>().style.color = Color.white;
            ActorField.style.color = Color.white;
            ActorField.value = null;
            ContentElement.Add(ActorField);

            DialogField = new();
            DialogField.verticalScrollerVisibility = ScrollerVisibility.Auto;
            DialogField.style.whiteSpace = WhiteSpace.Normal;
            DialogField.label = "Dialog:";
            DialogField.Q<Label>().style.color = Color.white;
            DialogField.value = "";
            DialogField.style.color = Color.white;
            DialogField.multiline = true;
            ContentElement.Add(DialogField);

            ScreenEventsField = new();
            ScreenEventsField.label = "Screen Actions:";
            ScreenEventsField.style.color = Color.white;
            ScreenEventsField.pickingMode = PickingMode.Ignore;
            ScreenEventsField.focusable = false;
            ContentElement.Add(ScreenEventsField);

            EndGapSpace = new();
            EndGapSpace.style.height = 20;
            ContentElement.Add(EndGapSpace);
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
                    value.isExpanded = false;
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