#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
namespace UITK_SimpleTimeline.Editor
{
    public class SimpleTimeline_EditorWindow : EditorWindow
    {
        public static SimpleTimeline_EditorWindow WindowInstance;
        public SimpleTimeline TimelineToEdit = new();

        private SimpleTimelineUITKField timelineField;
        
        public static void OpenWindow(SimpleTimeline givenTimeline)
        {
            if (WindowInstance != null) { Debug.LogWarning("Simple Timeline Editor Window already open!"); return; }
            WindowInstance = GetWindow<SimpleTimeline_EditorWindow>();
            WindowInstance.TimelineToEdit = givenTimeline;
            if (givenTimeline.Curves == null) WindowInstance.TimelineToEdit.Curves = new();
            WindowInstance.titleContent = new GUIContent("Simple Timeline Editor");
        }

        public static void OpenWindow(SerializedProperty sp, SerializedObject windObject)
        {
            if(WindowInstance != null) { Debug.LogWarning("Simple Timeline Editor Window already open!"); return; }
            WindowInstance = GetWindow<SimpleTimeline_EditorWindow>();
            Helper.SimpleTimelineProperty = sp;
            Helper.WindowObject = windObject;
            WindowInstance.TimelineToEdit = (SimpleTimeline)sp.boxedValue;
            WindowInstance.titleContent = new GUIContent("Simple Timeline Editor");
        }

        public static void OpenWindow()
        {
            if (WindowInstance != null) { Debug.LogWarning("Simple Timeline Editor Window already open!"); return; }
            WindowInstance = GetWindow<SimpleTimeline_EditorWindow>();
            WindowInstance.TimelineToEdit = new();
            WindowInstance.TimelineToEdit.Curves = new();
            WindowInstance.titleContent = new GUIContent("Simple Timeline Editor");
        }


        public void CreateGUI()
        {
            if(Helper.WindowObject == null) Helper.WindowObject = new(this);
            if(Helper.SimpleTimelineProperty == null) Helper.SimpleTimelineProperty = Helper.WindowObject.FindProperty("TimelineToEdit");
            //Debug.Log("SimpleTimelineProp: " + Helper.SimpleTimelineProperty.name);
            Helper.CurvesProperty = Helper.SimpleTimelineProperty.FindPropertyRelative("Curves");
            //Debug.Log("Curve Prop: " + Helper.CurvesProperty.name);
            timelineField = new("", TimelineToEdit);
            
            rootVisualElement.Add(timelineField);
        }

        private void OnDestroy()
        {
            //super duper make sure any changes made to the stinkin' simpletimeline are saved!
            Debug.Log("Applying changes to SerializedO: " + Helper.WindowObject.targetObject.name);
            Helper.WindowObject.ApplyModifiedProperties();
            Helper.SimpleTimelineProperty = null;
            Helper.WindowObject = null;
            WindowInstance = null;
            timelineField = null;
        }
    }
}
#endif