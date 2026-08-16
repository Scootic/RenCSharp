#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    public class SimpleTimeline_EditorWindow : EditorWindow
    {
        public static SimpleTimeline_EditorWindow windowInstance;

        private SimpleTimelineUITKField timelineField;
        private VisualElement root;
        private static SimpleTimeline timelineToEdit = new();

        private readonly string uitkDocumentFilePath = "Assets/UITK_SimpleTimeline/Scripts/Editor/SimpleTimeline_EditorWindow.uxml";
        private VisualTreeAsset uitkDocumentAsset;

        public static void OpenWindow(SimpleTimeline givenTimeline)
        {
            timelineToEdit = givenTimeline;
            windowInstance = GetWindow<SimpleTimeline_EditorWindow>();
            windowInstance.titleContent = new GUIContent("Simple Timeline Editor");
        }

        public void CreateGUI()
        {
            uitkDocumentAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uitkDocumentFilePath);
            try
            {
                root = uitkDocumentAsset.CloneTree();
            }
            catch
            {
                Debug.LogError($"SimpleTimeline Editor Window couldn't find the UITK Tree File at: {uitkDocumentFilePath}." +
                    $"Either you moved or deleted it. Too bad!");
                return;
            }

            timelineField = new("",timelineToEdit);
            root.Add(timelineField);

            rootVisualElement.Add(root);
        }
    }
}
#endif