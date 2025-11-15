using UnityEditor;
using UnityEngine;

namespace RenCSharp.Sequences
{
    [CustomEditor(typeof(Screen))]
    public class Screen_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            DrawDefaultInspector();
            Sequence selected = target as Sequence;
            Screen[] myTarget = selected.Screens;
        }
    }
}
