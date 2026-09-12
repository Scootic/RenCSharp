#if UNITY_EDITOR
using UnityEngine.UIElements;
namespace RenCSharp.Sequences.Editor
{
    public class FlagConditionField : BaseField<FlagCondition>
    {
        private readonly VisualElement ContentElement;
        private readonly TextField FlagToCheckField;
        private readonly IntegerField DesiredValueField;
        private readonly EnumField OperatorField;
        private readonly Toggle PersistentField, DebugResultField, BitwiseField;

        public FlagConditionField(string labelText) : base(labelText, new VisualElement())
        {
            ContentElement = this.Q<VisualElement>(className: inputUssClassName);
            ContentElement.style.flexDirection = FlexDirection.Column;
            labelElement.style.marginBottom = 1;

            FlagToCheckField = new TextField("Flag To Check:");
            ContentElement.Add(FlagToCheckField);

            DesiredValueField = new IntegerField("Desired Value:");
            DesiredValueField.tooltip = "This is the right side of the operation.";
            ContentElement.Add(DesiredValueField);

            OperatorField = new EnumField("Operation:", ConditionalOperator.Equals);
            OperatorField.tooltip = "Bitwise operations only support Equals/NotEquals";
            ContentElement.Add(OperatorField);

            PersistentField = new Toggle("Persistent:");
            PersistentField.tooltip = "Decides which dictionary of flags to check, per save if false, persistent if true.";
            ContentElement.Add(PersistentField);

            BitwiseField = new Toggle("Bitwise:");
            BitwiseField.tooltip = "Decides if the operation should be bitwise or not. Only supports Equals/NotEquals if true.";
            ContentElement.Add(BitwiseField);

            DebugResultField = new Toggle("DebugResult:");
            ContentElement.Add(DebugResultField);
        }

        public FlagCondition SetFlagCondition
        {
            set
            {
                FlagToCheckField.value = value.FlagToCheckProperty;
                DesiredValueField.value = value.DesiredValue;
                OperatorField.value = value.Operation;
                PersistentField.value = value.Persistent;
                BitwiseField.value = value.BitWise;
                DebugResultField.value = value.DebugResult;
            }
        }

        public FlagCondition GetFlagCondition
        {
            get
            {
                FlagCondition fc = new();

                fc.DesiredValue = DesiredValueField.value;
                fc.FlagToCheckProperty = FlagToCheckField.value;
                fc.Operation = (ConditionalOperator)OperatorField.value;
                fc.Persistent = PersistentField.value;
                fc.BitWise = BitwiseField.value;
                fc.DebugResult = DebugResultField.value;

                return fc;
            }
        }
    }
}
#endif