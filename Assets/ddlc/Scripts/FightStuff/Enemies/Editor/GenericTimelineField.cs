#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;
namespace RenCSharp.Combat.Enemies
{
    public abstract class GenericTimelineField<T> : VisualElement where T : class
    {
        protected readonly VisualElement BackgroundElement;
        protected readonly VisualElement SelectableElement;
        protected readonly VisualElement PercentProgressVisual;
        protected readonly VisualElement InspectorElementBackground;
        //for editting the properties of the knob elements
        protected readonly PropertyField InspectorPropertyField;
        protected readonly UnsignedIntegerField CurrentSubdivisionField;
        protected readonly FloatField SecondsPerFrameField;
        protected readonly FloatField TimelineLength;
        protected readonly Button AddKnobButton;

        protected readonly Dictionary<string,GenericTimelineKnob<T>> ElementsDictionary;

        public GenericTimelineField() : this(null) { }

        public GenericTimelineField(VisualElement t) : base()
        {
            BackgroundElement = this.Q<VisualElement>();

            AddKnobButton.clicked += () =>
            {
                //GenericTimelineKnob<T> knob = new();
                string key = $"{CurrentSubdivisionField.value}";
                int i = 0;
                while (ElementsDictionary.ContainsKey(key))
                {

                    key = $"{CurrentSubdivisionField.value} - {i}";
                    i++;
                }
                //ElementsDictionary.Add(key, knob);
            };

            CurrentSubdivisionField.RegisterValueChangedCallback(evt => 
            {
                float perc = (float)CurrentSubdivisionField.value / (TimelineLength.value / SecondsPerFrameField.value);
                float newX = SelectableElement.style.width.value.value * perc;
                PercentProgressVisual.transform.position = new Vector3(newX, 0, 0);
            });
        }
    }

    public abstract class GenericTimelineKnob<T> : VisualElement where T : class
    {
        protected readonly VisualElement BackgroundElement;

        public GenericTimelineKnob() : this(null) { }

        public GenericTimelineKnob(string labelText) : base()
        {
            BackgroundElement = this.Q<VisualElement>();

            BackgroundElement.RegisterCallback<ClickEvent>(TheVent);
        }

        public Action OnSelect; //yell information to the InspectorField
        protected void TheVent(ClickEvent evt)
        {
            OnSelect?.Invoke();
        }
    }
}
#endif