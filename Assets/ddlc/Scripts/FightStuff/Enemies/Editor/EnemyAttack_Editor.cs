#if UNITY_EDITOR
using RenCSharp.EXPERIMENTAL;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace RenCSharp.Combat.Enemies.Editor
{
    [CustomEditor(typeof(EnemyAttackTimelineData))]
    public class EnemyAttackTimelineData_Editor : UnityEditor.Editor
    {
        private EnemyAttackTimelineData _me;

        private SerializedProperty arenaDimensionsProperty, controlTypeProperty, attackDurationProperty, projectileSpawnMethodProperty, projectileIndexMethodProperty, projectilesThatSpawnProperty;
        private SerializedProperty keyframesProperty, frameDataProperty, loopsProperty;

        private void OnEnable()
        {
            _me = target as EnemyAttackTimelineData;
            SerializedObject so = new SerializedObject(_me);
            arenaDimensionsProperty = so.FindProperty("arenaDimensions");
            controlTypeProperty = so.FindProperty("controlType");
            attackDurationProperty = so.FindProperty("attackDuration");
            projectileSpawnMethodProperty = so.FindProperty("projectileSpawnPositionMethod");
            projectileIndexMethodProperty = so.FindProperty("projectileIndexMethod");
            projectilesThatSpawnProperty = so.FindProperty("projectilesThatSpawn");
            keyframesProperty = so.FindProperty("keyframes");
            frameDataProperty = so.FindProperty("frameData");
            loopsProperty = so.FindProperty("loops");
        }

        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open In Timeline Editor"))
            {
                EnemyAttackTimeLineEditor.DoWindow(_me);
            }

            EditorGUILayout.LabelField("Default Enemy Attack Data");
            EditorGUILayout.PropertyField(arenaDimensionsProperty);
            EditorGUILayout.PropertyField(controlTypeProperty);
            EditorGUILayout.PropertyField(attackDurationProperty);
            EditorGUILayout.PropertyField(projectilesThatSpawnProperty);
            EditorGUILayout.PropertyField(projectileSpawnMethodProperty);
            EditorGUILayout.PropertyField(projectileIndexMethodProperty);

            EditorGUILayout.LabelField("Timeline Data");
            EditorGUILayout.PropertyField(loopsProperty);
            EditorGUILayout.PropertyField(keyframesProperty);
            EditorGUILayout.PropertyField(frameDataProperty);
        }
    }

    [CustomEditor(typeof(EnemyAttack))]
    public class EnemyAttack_Editor : UnityEditor.Editor
    {
        private EnemyAttack _me;
        private float _radius;
        private byte _resolution;
        /// <summary>
        /// String that will be converted into the radius value of the circle.
        /// </summary>
        private string _radString;
        /// <summary>
        /// String that will be converted into the resolution value of the circle. (How many spawn positions make up the circle.)
        /// </summary>
        private string _resString;

        private void OnEnable()
        {
            _me = target as EnemyAttack;
        }

        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open In Timeline Editor"))
            {
                EnemyAttackTimeLineEditor.DoWindow(_me);
            }
            base.OnInspectorGUI();
            GUILayout.Label("Radius");
            _radString = GUILayout.TextField(_radString, GUILayout.Width(150));
            bool f = float.TryParse(_radString, out _radius);
            GUILayout.Label("Resolution");
            _resString = GUILayout.TextField(_resString, GUILayout.Width(50));
            bool b = byte.TryParse(_resString, out _resolution);

            if(GUILayout.Button("Circle Toward Center"))
            {
                if (!f)
                {
                    Debug.LogError("Failed to Parse Radius");
                    return;
                }

                if (!b)
                {
                    Debug.LogError("Failed to Parse Resolution");
                    return;
                }

                List<Vector3> newSP = new();
                List<Vector3> newID = new();

                for(int i = 0; i < _resolution; i++)
                {
                    float percAlong = (float)i / (float)_resolution;
                    percAlong *= TrigHelper.TAU;

                    Vector3 newSPos = new Vector3(Mathf.Cos(percAlong), Mathf.Sin(percAlong), 0);
                    Vector3 newIDir = new Vector3(newSPos.x * -1, newSPos.y * -1, 0);
                    newSPos *= _radius;

                    newSP.Add(newSPos);
                    newID.Add(newIDir);
                }

                _me.OverrideSpawnPoints(newSP, newID);
                EditorUtility.SetDirty(_me);
            }
        }
    }
}
#endif