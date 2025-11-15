using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
namespace RenCSharp.Sequences
{
    [CustomEditor(typeof(Sequence))]
    public class Sequence_Editor : Editor
    {
        private Assembly childrenOfSE = Assembly.GetAssembly(typeof(Screen_Event));
        private Type[] allSubs => childrenOfSE.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(Screen_Event))).ToArray();
        string screenIndex = "0";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Sequence selected = target as Sequence;
            Screen[] myTarget = selected.Screens;
            GUILayout.Label("Screen Index");
            screenIndex = GUILayout.TextField(screenIndex, 3);
            GUILayout.Label("Possible Screen Actions");
            foreach (Type stupid in allSubs) //nested AF! HORRID!
            {
                if (GUILayout.Button(stupid.ToString()))
                {
                    if (int.TryParse(screenIndex, out int result) && result >= 0)
                    {
                        if (result < myTarget.Length)
                        {
                            myTarget[result].ScreenActions.Add(Activator.CreateInstance(stupid) as Screen_Event);
                        }
                        else
                        {
                            Debug.LogWarning("Screen Index too big! Out of bounds!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Bad screen index assigned! (0 through 999 please)");
                    }
                }
            }
        }
    }
}
