#if UNITY_EDITOR
using EXPERIMENTAL;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace RenCSharp.Combat.Enemies
{
    [CustomEditor(typeof(EnemyAttack))]
    public class EnemyAttack_Editor : Editor
    {
        private EnemyAttack _me;
        private float _radius;
        private byte _resolution;
        private string _radString;
        private string _resString;

        private void OnEnable()
        {
            _me = target as EnemyAttack;
        }

        public override void OnInspectorGUI()
        {
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