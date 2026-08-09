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
        private RenConsole_InputField consoleInput;
        private bool open;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            open = true;
            uiRoot = consoleDocument.rootVisualElement;
            consoleLogs = uiRoot.Q<ListView>();
            consoleInput = uiRoot.Q<RenConsole_InputField>();
            consoleInput.RegisterValueChangedCallback(ReceiveCommand);
            consoleInput.isDelayed = true;
        }

        private void OnEnable()
        {
            Button_Inputs.OpenConsole += FlipConsole;
            RenConsole.UpdateLogsListView += UpdateLogsView;
        }

        private void OnDisable()
        {
            Button_Inputs.OpenConsole -= FlipConsole;
            RenConsole.UpdateLogsListView -= UpdateLogsView;
            consoleInput.UnregisterValueChangedCallback(ReceiveCommand);
        }

        // Update is called once per frame
        void UpdateLogsView()
        {
            if (consoleLogs == null) return;
            consoleLogs.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            consoleLogs.itemsSource = RenConsole.ConsoleLogs;
            consoleLogs.makeItem = () => new RenConsole_LogField();
            consoleLogs.bindItem = (VisualElement e, int i) =>
            {
                RenConsole_LogField logField = e as RenConsole_LogField;
                logField.SetLabel(RenConsole.ConsoleLogs[i].DateTime);
                logField.SetMessage(RenConsole.ConsoleLogs[i].Message);
                logField.SetSeverity(RenConsole.ConsoleLogs[i].Severity);
            };
            consoleLogs.selectionType = SelectionType.None;
            consoleLogs.ScrollToItem(RenConsole.ConsoleLogs.Length - 1);
        }

        void FlipConsole()
        {
            open = !open;
            //play anim?
            if (open)
            {
                consoleInput.RemoveFromClassList("renCSharp-console-input:enabled");
                consoleInput.AddToClassList("renCSharp-console-input");
                consoleLogs.RemoveFromClassList("renCSharp-console-logs:enabled");
                consoleLogs.AddToClassList("renCSharp-console-logs");
                consoleInput.UnregisterValueChangedCallback(ReceiveCommand);
                consoleInput.Blur();
                consoleInput.focusable = false;
            }
            else
            {
                consoleInput.RemoveFromClassList("renCSharp-console-input");
                consoleInput.AddToClassList("renCSharp-console-input:enabled");
                consoleLogs.RemoveFromClassList("renCSharp-console-logs");
                consoleLogs.RemoveFromClassList("renCSharp-console-logs:enabled");
                UpdateLogsView();
                consoleInput.RegisterValueChangedCallback(ReceiveCommand);
                consoleInput.focusable = true;
                consoleInput.Focus();
            }
        }

        void ReceiveCommand(ChangeEvent<string> input)
        {
            RenConsole.ParseCommand(input.newValue);
            consoleInput.SetValueWithoutNotify("...");
        }
    }
}
