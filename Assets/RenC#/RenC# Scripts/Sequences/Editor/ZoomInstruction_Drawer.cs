#if UNITY_EDITOR
using RenCSharp.Actors;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
namespace RenCSharp.Sequences.Editor
{
    [CustomPropertyDrawer(typeof(ZoomInstruction))]
    public class ZoomInstruction_Drawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement toReturn = new();

            ObjectField actorField = new("Actor To Focus On:")
            {
                objectType = typeof(Actor),
                value = property.FindPropertyRelative("ActorToFocus").boxedValue as Actor,
                tooltip = "Overrides position instructions, zoom in on the Actor's position instead. Leave empty for a set screen position (like 0,0,0 for origin)."
            };
            actorField.style.flexDirection = FlexDirection.Column;
            actorField.style.height = 35;
            actorField.Children().ToArray()[1].style.flexBasis = 18;
            actorField.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("ActorToFocus").boxedValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            FloatField xPos = new("X Position:")
            {
                value = property.FindPropertyRelative("xPos").floatValue
            };
            xPos.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("xPos").floatValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            xPos.style.flexDirection = FlexDirection.Column;
            FloatField yPos = new("Y Position:")
            {
                value = property.FindPropertyRelative("yPos").floatValue
            };
            yPos.style.flexDirection = FlexDirection.Column;
            yPos.RegisterValueChangedCallback(evt => { property.FindPropertyRelative("yPos").floatValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            FloatField zPos = new("Z Position:")
            {
                value = property.FindPropertyRelative("zPos").floatValue
            };
            zPos.style.flexDirection = FlexDirection.Column;
            zPos.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("zPos").floatValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            FloatField zoomScale = new("Zoom Scale:")
            {
                value = property.FindPropertyRelative("zoomScale").floatValue
            };
            zoomScale.style.flexDirection = FlexDirection.Column;
            zoomScale.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("zoomScale").floatValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            toReturn.TrackPropertyValue(property, callback =>
            {
                actorField.SetValueWithoutNotify(property.FindPropertyRelative("ActorToFocus").boxedValue as Actor);
                xPos.SetValueWithoutNotify(property.FindPropertyRelative("xPos").floatValue);
                yPos.SetValueWithoutNotify(property.FindPropertyRelative("yPos").floatValue);
                zPos.SetValueWithoutNotify(property.FindPropertyRelative("zPos").floatValue);
                zoomScale.SetValueWithoutNotify(property.FindPropertyRelative("zoomScale").floatValue);
            });

            toReturn.Add(actorField);
            toReturn.Add(xPos);
            toReturn.Add(yPos);
            toReturn.Add(zPos);
            toReturn.Add(zoomScale);

            return toReturn;
        }
    }
}
#endif