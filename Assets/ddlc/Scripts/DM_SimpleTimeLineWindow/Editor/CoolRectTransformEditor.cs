using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DMTimeArea
{
    
    [CustomEditor(typeof(RectTransform))]
    [CanEditMultipleObjects]
    public class CoolRectTransformEditor : Editor
    {
        private Editor _originalEditor;
        private List<RectTransform> _me;

        private void OnEnable()
        {
            System.Type internalEditorType = typeof(Editor).Assembly.GetType("UnityEditor.RectTransformEditor");
            _originalEditor = CreateEditor(targets, internalEditorType);
        }
     
        public override void OnInspectorGUI()
        {
            _me = new();
            foreach(Object obj in targets)
            {
                if (obj is RectTransform rt) _me.Add(rt);
            }
            //base.OnInspectorGUI();
            _originalEditor.OnInspectorGUI();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Vector3Field("Transform Position", _me[0].position);
            EditorGUILayout.Vector3Field("Transform Local Position", _me[0].localPosition);
            EditorGUI.EndDisabledGroup();
        }
    }
}
