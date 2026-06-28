using UnityEngine;
using UnityEngine.UIElements;
namespace RenCSharp.EXPERIMENTAL
{
    /// <summary>
    /// GameObject component for interacting with the RenConsole during runtime.
    /// </summary>
    public class RenConsole_Object : MonoBehaviour
    {
        [SerializeField] private UIDocument consoleDocument;

        private VisualElement uiRoot;
        private ListView consoleLogs;
        private RenConsole_TextField consoleInput;
        private bool open;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            open = false;
            uiRoot = consoleDocument.rootVisualElement;
            consoleLogs = uiRoot.Q<ListView>();
            consoleInput = uiRoot.Q<RenConsole_TextField>();
        }

        private void OnEnable()
        {
            Button_Inputs.OpenConsole += FlipConsole;
        }

        private void OnDisable()
        {
            Button_Inputs.OpenConsole -= FlipConsole;
        }

        // Update is called once per frame
        void UpdateLogsView()
        {
        
        }

        void FlipConsole()
        {
            open = !open;
            consoleDocument.gameObject.SetActive(open);
            //play anim?
        }
    }
}
