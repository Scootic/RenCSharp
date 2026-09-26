#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using RenCSharp.Editor;
using RenCSharp.Actors;
using UnityEditor.UIElements;
namespace RenCSharp.Sequences.Editor
{
    [CustomPropertyDrawer(typeof(VisualIndexes))]
    public class VisualIndexes_Drawer : PropertyDrawer
    {
        private AutoTextField[] autoTextFields;
        private SerializedProperty viProperty;
        private ObjectField actorField;
        private VisualElement leElement;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            viProperty = property;
            leElement = new() { name = "VisualIndex Property Element"};

            actorField = new()
            {
                objectType = typeof(Actor),
                value = viProperty.FindPropertyRelative("ActorToSet").boxedValue as Actor
            };
            actorField.RegisterValueChangedCallback(evt => 
            {
                VisualIndexes vi = (VisualIndexes)viProperty.boxedValue;
                vi.SetActor = evt.newValue as Actor;
                viProperty.boxedValue = vi;
                viProperty.serializedObject.ApplyModifiedProperties(); //works...?
                SetAutoTextFields();
            });

            leElement.Add(actorField);
            if (actorField.value != null)
            {
                SetAutoTextFields();
            }
            //so that if the VisualIndexes property itself changes, override autotext fields.
            ///?!?!?!?!?!?!? absolute hogwash that don't work
            /*leElement.TrackPropertyValue(viProperty, evt =>
            {
                actorField.value = evt.FindPropertyRelative("ActorToSet").boxedValue as Actor; //????
                SetAutoTextFields();
            });*/

            return leElement;
        }

        private void SetAutoTextFields()
        {
            //remove any existing autotextfields for being complete and utter hogwash
            for (int i = leElement.childCount - 1; i >= 1; i--)
            {
                leElement.RemoveAt(i);
            }
            try
            {
                int length = ((Actor)actorField.value).Visuals.Length;
                autoTextFields = new AutoTextField[length];
            }
            catch { Debug.LogError("Failed to get Actor Visuals length?"); return; }
            List<List<string>> AutoTexts = ((VisualIndexes)viProperty.boxedValue).GetAutoTexts;
            if (viProperty.FindPropertyRelative("indexes").arraySize != AutoTexts.Count)
            {
                viProperty.FindPropertyRelative("indexes").arraySize = AutoTexts.Count;
                viProperty.serializedObject.ApplyModifiedProperties();
            }
            for (int i = 0; i < autoTextFields.Length; i++)
            {
                int oldI = i;
                autoTextFields[oldI] = new AutoTextField($"Layer {oldI}:", AutoTexts[oldI], FlexDirection.Column);
                autoTextFields[oldI].RegisterValueChangedCallback(evt => 
                {
                    VisualIndexes vi = (VisualIndexes)viProperty.boxedValue;
                    vi.indexes[oldI] = evt.newValue;
                    viProperty.boxedValue = vi;
                    viProperty.serializedObject.ApplyModifiedProperties();
                }
                );
                autoTextFields[oldI].SetText = viProperty.FindPropertyRelative("indexes").GetArrayElementAtIndex(oldI).stringValue;
                
                leElement.Add(autoTextFields[oldI]);
            }
        }
    }
}
#endif