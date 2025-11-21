using UnityEngine;

namespace RenCSharp
{
    public struct FlagCondition
    {
        [SerializeField] private string flagToCheck;
        [SerializeField] private ConditionalOperator operation;
        [SerializeField] private int desiredValue;

        public bool ConditionMet()
        {
            bool met = false;
            int foundVal = Script_Manager.SM.GetFlag(flagToCheck);

            switch (operation)
            {
                case ConditionalOperator.Equals:
                    if (foundVal == desiredValue) met = true;
                    break;
                case ConditionalOperator.GreaterThan:
                    if (foundVal > desiredValue) met = true;
                    break;
                    case ConditionalOperator.LessThan:
                    if (foundVal < desiredValue) met = true;
                    break;
                case ConditionalOperator.GreaterThanEquals:
                    if (foundVal >= desiredValue) met = true;
                    break;
                case ConditionalOperator.LessThanEquals:
                    if (foundVal <= desiredValue) met = true;
                    break;
            }

            return met;
        }
    }
}
