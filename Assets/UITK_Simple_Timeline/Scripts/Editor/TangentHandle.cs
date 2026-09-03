#if UNITY_EDITOR
using UnityEditor;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
namespace UITK_SimpleTimeline.Editor
{
    public class TangentHandle : VisualElement
    {
        private static Texture2D handleBase, handleConnector, handleEnd;

        private readonly VisualElement baseElement, connectorElement, endElement;
        private readonly float initialRotation, maxRotation;

        public float GetRotationAngle => baseElement.style.rotate.value.angle.value;
        /// <summary>
        /// Always invoked after the user rotates the element through dragging.
        /// </summary>
        public Action<float> GiveCurAngle;

        /// <summary>
        /// New tangent handle, please :)
        /// </summary>
        /// <param name="positionOffset">X is dist from left, Y is dist from top. Absolute position.</param>
        /// <param name="initialRotationAngle">Positive rotations are clockwise.</param>
        /// <param name="maximumValidRotation">Clamps rotations compared to the initial rotation, in both directions.</param>
        public TangentHandle(Vector2 positionOffset, float initialRotationAngle, float maximumValidRotation = 90f) 
        {
            initialRotation = initialRotationAngle;
            maxRotation = maximumValidRotation;

            if (handleBase == null) handleBase = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath + "/handlebase.png");
            if (!handleBase) Debug.LogWarning("TangentHandle.cs can't find handlebase.png. Make sure the SimpleTimelineUITK_Helper.EditorIconAssetPath matches your project.");
            if (handleConnector == null) handleConnector = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath + "/handleconnector.png");
            if (!handleConnector) Debug.LogWarning("TangentHandle.cs can't find handleconnector.png. Make sure the SimpleTimelineUITK_Helper.EditorIconAssetPath matches your project.");
            if (handleEnd == null) handleEnd = AssetDatabase.LoadAssetAtPath<Texture2D>(Helper.EditorIconAssetPath + "/handleend.png");
            if (!handleEnd) Debug.LogWarning("TangentHandle.cs can't find handleend.png. Make sure the SimpleTimelineUITK_Helper.EditorIconAssetPath matches your project.");

            baseElement = this;
            baseElement.style.backgroundImage = handleBase;
            baseElement.style.width = 25f;
            baseElement.style.height = 25f;
            baseElement.style.position = Position.Absolute;
            baseElement.style.left = positionOffset.x; 
            baseElement.style.top = positionOffset.y;
            baseElement.style.transformOrigin = new TransformOrigin(12.5f, 12.5f);
            baseElement.style.rotate = new Rotate(initialRotation);

            connectorElement = new();
            connectorElement.style.backgroundImage = handleConnector;
            connectorElement.style.width = 75f;
            connectorElement.style.height = 25f;
            connectorElement.style.left = 37.5f;
            baseElement.Add(connectorElement);

            endElement = new();
            endElement.style.backgroundImage = handleEnd;
            endElement.style.width = 25f;
            endElement.style.height = 25f;
            endElement.style.left = 12.5f;
            endElement.RegisterCallback<PointerMoveEvent>(CommitRotation);
            connectorElement.Add(endElement);
        }

        private void CommitRotation(PointerMoveEvent pme)
        {
            if ((pme.pressedButtons & 1) != 1) return;

            Debug.Log($"MouseLoc: {pme.localPosition}");
            float yLoc = pme.localPosition.y;
            float desAngle = Mathf.Clamp(GetRotationAngle + yLoc, initialRotation - maxRotation, initialRotation + maxRotation);

            baseElement.style.rotate = new Rotate(desAngle);
            GiveCurAngle?.Invoke(GetRotationAngle);
        }
    }
}
#endif