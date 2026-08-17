#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace UITK_SimpleTimeline.Editor
{
    public class SimpleTimeline_EditorWindow : EditorWindow
    {
        public static SimpleTimeline_EditorWindow windowInstance;

        private static SimpleTimelineUITKField timelineField;
        private static SimpleTimeline timelineToEdit = new();

        public static void OpenWindow(SimpleTimeline givenTimeline)
        {
            if (windowInstance != null) { Debug.LogWarning("Simple Timeline Editor Window already open!"); return; }
            timelineToEdit = givenTimeline;
            windowInstance = GetWindow<SimpleTimeline_EditorWindow>();
            windowInstance.titleContent = new GUIContent("Simple Timeline Editor");
        }

        public static void OpenWindow(SimpleTimeline st, SimpleTimelineUITKField field)
        {
            if (windowInstance != null) { Debug.LogWarning("Simple Timeline Editor Window already open!"); return; }
            timelineToEdit = st;
            timelineField = field;
            windowInstance = GetWindow<SimpleTimeline_EditorWindow>();
            windowInstance.titleContent = new GUIContent("Simple Timeline Editor");
        }

        public void CreateGUI()
        {
            //if no subclass SimpleTimelineUITKField is passed on creation, default to the base one. Sad and typeless!
            if(timelineField != null) timelineField = new("",timelineToEdit);

            rootVisualElement.Add(timelineField);
        }

        private void OnDestroy()
        {
            windowInstance = null;
            timelineToEdit = new();
            timelineField = null;
        }
    }
}
#endif