#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// I hate this so much. Try to give screen events their name, so you actually know what the hell they are.
    /// Also, gives a stupid ah dropdown to swap out event types within the sequence.
    /// </summary>
    [CustomPropertyDrawer(typeof(Screen_Event))]
    public class Screen_Event_Drawer : PolymorphicPropertyDrawer<Screen_Event>
    {
        protected override string DropDownMenuName()
        {
            return "Select Screen Event Type";
        }
    }

    [CustomEditor(typeof(Sequence)), CanEditMultipleObjects]
    public class Sequence_Editor : Editor
    {
        private Sequence _target;
        public override void OnInspectorGUI()
        {
            if (_target == null) _target = target as Sequence;

            if (GUILayout.Button("Calculate Sequence Length")) 
            {
                int totalScreens = 0;
                int wordcount = 0;
                long totalChars = 0;

                foreach(UnityEngine.Object obj in targets)
                {
                    _target = (Sequence)obj;
                    totalScreens += _target.Screens.Length;
                    foreach(Screen s in _target.Screens)
                    {
                        int i = 0;
                        char[] chars = s.Dialog.ToCharArray();
                        while (i < chars.Length)
                        {
                            while (i < chars.Length && !char.IsWhiteSpace(chars[i])) 
                            {
                                i++;
                                totalChars++;
                            }

                            wordcount++;

                            while (i < chars.Length && char.IsWhiteSpace(chars[i]))
                            {
                                i++;
                                totalChars++;
                            }
                        }
                    }
                }

                double totalSeconds = ((double)totalChars / 60d) + (totalScreens * 0.5f); //total characters divided by 60FPS (a char a frame) +
                                                                                          //linger time of 0.5seconds per screen.
                double totalMinutes = totalSeconds / 60d;
                double totalHours = totalMinutes / 60d;

                totalHours = Mathf.Floor((float)totalHours);
                totalMinutes -= totalHours * 60d;
                totalMinutes = Mathf.Floor((float) totalMinutes);
                totalSeconds -= totalMinutes * 60d;
                totalSeconds = Mathf.Floor((float)totalSeconds);

                Debug.Log($"Total Found Screens: {totalScreens}." +
                    $"\nTotal Found Characters: {totalChars}. Total Word Count: {wordcount}" +
                    $"\nAssuming 60FPS and left on auto at fastest possible speed (linger time of 0.5s), the total time *should* be:" +
                    $"\n{totalHours}:{totalMinutes}:{totalSeconds} (H:M:S). (Ignores fade transitions and other pauses to sequence.)");
            }
            
            if (GUILayout.Button("Replace Deprecated Events")) //pretty please update to include things you want to replace en masse
            {
                foreach (Object obj in targets)
                {
                    _target = (Sequence)obj;
                    int replaced = 0;
                    for (int i = 0; i < _target.Screens.Length; i++) //run through every single screen
                    {
                        Screen s = _target.Screens[i];
                        for (int j = 0; j < s.ScreenActions.Count; j++)
                        {
                            Screen_Event vent = s.ScreenActions[j];
                            IDeprecatedReplaceable<Screen_Event> replacer = vent as IDeprecatedReplaceable<Screen_Event>;
                            if(replacer != null)
                            {
                                s.ScreenActions[j] = replacer.Replacement();
                                replaced++;
                            }
                        }
                    }
                    EditorUtility.SetDirty(obj);
                    Debug.Log("Done replacing " + _target.name + "'s deprecated events! Replaced: " + replaced + " events.");
                }
            }

            if (GUILayout.Button("Find any Null Asset References"))
            {
                foreach(Object obj in targets)
                {
                    _target = obj as Sequence;
                    int nullsFound = 0;
                    for(int i = 0; i < _target.Screens.Length; i++)
                    {
                        Screen s = _target.Screens[i];
                        for(int j = 0; j < s.ScreenActions.Count; j++)
                        {
                            if (s.ScreenActions[j] is INullAssetReferenceCheck)
                            {
                                INullAssetReferenceCheck narc = s.ScreenActions[j] as INullAssetReferenceCheck;
                                if (narc.HasNullAssetReferences())
                                {

                                    Debug.Log($"Null Asset Reference at: {_target.name} -> Screen: {i} -> Action: {j}.");
                                    nullsFound++;
                                }
                            }
                            if (s.ScreenActions[j].ToString() == "Conditional Screen") //the mostest absolutely worstest nesting in history
                            {
                                Conditional_Screen_Overrider jabroni = s.ScreenActions[j] as Conditional_Screen_Overrider;
                                foreach(ConditionalScreen cs in jabroni.PossibleScreens)
                                {
                                    Screen stinker = cs.ResultingScreen;
                                    for(int k = 0; k < stinker.ScreenActions.Count; k++)
                                    {
                                        if (stinker.ScreenActions[k] is INullAssetReferenceCheck)
                                        {
                                            INullAssetReferenceCheck horrid = stinker.ScreenActions[k] as INullAssetReferenceCheck;
                                            if (horrid.HasNullAssetReferences())
                                            {
                                                Debug.Log($"Null Asset Ref at: {_target.name} -> Screen: {i} -> conditional screen -> Resulting Screen{cs.ToString()} -> Screen Event: {k}.");
                                                nullsFound++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (nullsFound == 0) Debug.Log($"{_target.name} has no Null Asset References!");
                    else Debug.LogWarning($"{_target.name} has: {nullsFound} Null Asset References.");
                }
            }

            if (GUILayout.Button("Open in Sequence Editor"))
            {
                Sequence_EditorWindow.OpenWindow(_target);
                Sequence_EditorWindow.SetTarget = _target;
            }

            GUILayout.Label("It is highly recommended that you use the Sequence Editor Window instead of directly" +
                " editing\nthe sequence in the inspector. The Editor Window is more performant in situations with large\n" +
                "screen counts. Additionally, certain Editor Window functions don't function 'perfectly' when\nworking " +
                "with screens made in the inspector. You are still able to access the inspector to verify\nthat everything appears " +
                "to be in order.");

            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update(); //?
            }
        }
    }
}
#endif