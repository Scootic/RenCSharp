using UnityEngine;
using UnityEngine.UIElements;

namespace RenCSharp.EXPERIMENTAL
{
    [UxmlElement]
    public partial class RenConsole_LogField : VisualElement
    {
        private readonly VisualElement background;
        private readonly VisualElement gap, empty;
        private readonly Label labelElement;
        private readonly TextElement logElement;

        public RenConsole_LogField() : this("Log Time!", "Empty!", LogSeverity.Null) { }

        public void SetLabel(string label)
        {
            labelElement.text = label;
        }

        public void SetMessage(string msg)
        {
            logElement.text = msg;
        }

        public void SetSeverity(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.LogWarning:
                    background.style.backgroundColor = CoolColors.slightTransYellowGUI;
                    break;
                case LogSeverity.LogError:
                    background.style.backgroundColor = CoolColors.slightTransRedGUI;
                    break;
                case LogSeverity.LogPositive:
                    background.style.backgroundColor = CoolColors.slightTransBlueGUI;
                    break;
                default:
                    background.style.backgroundColor = CoolColors.slightTransGrayGUI;
                    break;
            }
        }

        public RenConsole_LogField(string label, string msg, LogSeverity severity)
        {
            background = this.Q<VisualElement>();
            background.style.flexDirection = FlexDirection.Column;
            background.style.whiteSpace = WhiteSpace.Normal;

            switch (severity)
            {
                case LogSeverity.LogWarning:
                    background.style.backgroundColor = CoolColors.slightTransYellowGUI;
                    break;
                case LogSeverity.LogError:
                    background.style.backgroundColor = CoolColors.slightTransRedGUI;
                    break;
                case LogSeverity.LogPositive:
                    background.style.backgroundColor = CoolColors.slightTransBlueGUI;
                    break;
                default:
                    background.style.backgroundColor = CoolColors.slightTransGrayGUI;
                    break;
            }

            gap = new VisualElement();
            background.Add(gap);
            gap.style.height = 5;
            gap.style.width = 1920;
            gap.style.backgroundColor = Color.black;
            gap.style.color = Color.black;

            empty = new VisualElement();
            background.Add(empty);
            empty.style.flexDirection = FlexDirection.Row;

            labelElement = new Label();
            empty.Add(labelElement);
            labelElement.style.color = Color.white;
            labelElement.text = label;

            logElement = new TextElement();
            empty.Add(logElement);
            logElement.text = msg;
            logElement.style.color = Color.white;
        }
    }
}
