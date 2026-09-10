#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
using RenCSharp.Editor;
using RenCSharp.Actors;
using UnityEditor.UIElements;
using UnityEngine;
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
            leElement = new() { name = "VisualIndex Property Element"};

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
            Debug.Log($"actortosetpropert{viProperty.FindPropertyRelative("ActorToSet")}");
            leElement.Add(actorField);
            try
            {
               
                autoTextFields = new AutoTextField[viProperty.FindPropertyRelative("ActorToSet").FindPropertyRelative("Visuals").arraySize];
            }
            catch
            {
                return leElement;
            }
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

            return leElement;
        }

        private void SetAutoTextFields()
        {
            Debug.Log("Setting auto text fields for the VisualIndexes drawer");
            //remove any existing autotextfields for being complete and utter hogwash
            for (int i = leElement.childCount - 1; i > 1; i--)
            {
                leElement.RemoveAt(i);
            }
            try
            {
                autoTextFields = new AutoTextField[viProperty.FindPropertyRelative("ActorToSet").FindPropertyRelative("Visuals").arraySize];
            }
            catch { return; }
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