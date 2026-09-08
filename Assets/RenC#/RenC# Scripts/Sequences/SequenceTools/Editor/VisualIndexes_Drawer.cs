#if UNITY_EDITOR
using UnityEditor;
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
        private VisualElement leElement;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            viProperty = property;
            leElement = new();

            ObjectField actorField = new()
            {
                objectType = typeof(Actor),
                value = viProperty.FindPropertyRelative("ActorToSet").boxedValue as Actor
            };
            actorField.RegisterValueChangedCallback(evt => 
            {
                VisualIndexes vi = (VisualIndexes)viProperty.boxedValue;
                vi.SetActor = evt.newValue as Actor;
                viProperty.boxedValue = vi;
                viProperty.serializedObject.ApplyModifiedProperties();
                SetAutoTextFields();
            });

            leElement.Add(actorField);

            autoTextFields = new AutoTextField[viProperty.FindPropertyRelative("indexes").arraySize];
            List<List<string>> AutoTexts = ((VisualIndexes)viProperty.boxedValue).GetAutoTexts;

            for(int i = 0; i < autoTextFields.Length; i++)
            {
                autoTextFields[i] = new AutoTextField($"Layer {i}:", AutoTexts[i]);
                autoTextFields[i].RegisterValueChangedCallback(evt => 
                {
                    viProperty.FindPropertyRelative("indexes").GetArrayElementAtIndex(i).stringValue = evt.newValue;
                    viProperty.serializedObject.ApplyModifiedProperties();
                }
                );
                leElement.Add(autoTextFields[i]);
            }

            return leElement;
        }

        private void SetAutoTextFields()
        {
            //remove any existing autotextfields for being complete and utter hogwash
            for (int i = leElement.childCount - 1; i > 1; i--)
            {
                leElement.RemoveAt(i);
            }

            autoTextFields = new AutoTextField[viProperty.FindPropertyRelative("indexes").arraySize];
            List<List<string>> AutoTexts = ((VisualIndexes)viProperty.boxedValue).GetAutoTexts;

            for (int i = 0; i < autoTextFields.Length; i++)
            {
                autoTextFields[i] = new AutoTextField($"Layer {i}:", AutoTexts[i]);
                autoTextFields[i].RegisterValueChangedCallback(evt => 
                {
                    viProperty.FindPropertyRelative("indexes").GetArrayElementAtIndex(i).stringValue = evt.newValue;
                    viProperty.serializedObject.ApplyModifiedProperties();
                }
                );
                leElement.Add(autoTextFields[i]);
            }
        }
    }
}
#endif