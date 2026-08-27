#if UNITY_EDITOR
using UnityEngine;
using static UnityEditor.AnimationUtility;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    [CustomPropertyDrawer(typeof(TimelineKeyframe<>))]
    public class TimelineKeyframe_PropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement ve = new();
            ve.style.flexDirection = FlexDirection.Column;
            ve.style.left = 0;
            ve.style.right = 0;
            ve.style.bottom = 0;
            ve.style.top = 0;

            FloatField timeField = new("Time:");
            ve.Add(timeField);
            timeField.value = property.FindPropertyRelative("Time").floatValue;
            timeField.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("Time").floatValue = evt.newValue;
            });

            PropertyField valueField = new();
            valueField.BindProperty(property.FindPropertyRelative("Value"));
            ve.Add(valueField);

            FloatField inSlopeField = new("In Slope:");
            inSlopeField.value = property.FindPropertyRelative("InSlope").floatValue;
            inSlopeField.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("InSlope").floatValue = evt.newValue;
            });
            ve.Add(inSlopeField);

            FloatField outSlopeField = new("Out Slope:");
            outSlopeField.value = property.FindPropertyRelative("OutSlope").floatValue;
            outSlopeField.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("OutSlope").floatValue = evt.newValue;
            });
            ve.Add(outSlopeField);

            EnumField tangentModeField = new("Tangent Mode:",TangentMode.Free);
            tangentModeField.value = (TangentMode)property.FindPropertyRelative("TangentMode").enumValueIndex;
            tangentModeField.RegisterValueChangedCallback(evt =>
            {
                TangentMode noob = (TangentMode)evt.newValue;
                property.FindPropertyRelative("TangentMode").enumValueIndex = (int)noob;
            });
            ve.Add(tangentModeField);

            FloatField inTangentField = new("In Tangent:");
            inTangentField.value = property.FindPropertyRelative("InTangent").floatValue;
            inTangentField.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("InTangent").floatValue = evt.newValue;
            });
            ve.Add(inTangentField);

            FloatField outTangentField = new("Out Tangent:");
            outTangentField.value = property.FindPropertyRelative("OutTangent").floatValue;
            outTangentField.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("OutTangent").floatValue = evt.newValue;
            });
            ve.Add(outTangentField);

            EnumField weightModeField = new("Weighted Mode", WeightedMode.None);
            weightModeField.value = (WeightedMode)property.FindPropertyRelative("WeightedMode").enumValueIndex;
            weightModeField.RegisterValueChangedCallback(evt =>
            {
                WeightedMode noob = (WeightedMode)evt.newValue;
                property.FindPropertyRelative("WeightedMode").enumValueIndex = (int)noob;
            });
            ve.Add(weightModeField);

            FloatField inWeightField = new("In Weight");
            inWeightField.value = property.FindPropertyRelative("InWeight").floatValue;
            inWeightField.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("InWeight").floatValue = evt.newValue;
            });
            ve.Add(inWeightField);

            FloatField outWeightField = new("Out Weight");
            outWeightField.value = property.FindPropertyRelative("OutWeight").floatValue;
            outWeightField.RegisterValueChangedCallback(evt =>
            {
                property.FindPropertyRelative("OutWeight").floatValue = evt.newValue;
            });
            ve.Add(outWeightField);

            return ve;
        }
    }
}
#endif