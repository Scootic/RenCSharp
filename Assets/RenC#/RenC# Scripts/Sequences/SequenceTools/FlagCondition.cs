using System;
using UnityEngine;

namespace RenCSharp
{
    /// <summary>
    /// Conditions that check if the flag is the kind of value you want.
    /// </summary>
    [Serializable]
    public struct FlagCondition
    {
        [SerializeField] private string flagToCheck;
        [SerializeField, Tooltip("The found value is the left side of the operation.")] private ConditionalOperator operation;
        [SerializeField] private int desiredValue;
        [Header("BITWISE")]
        [SerializeField, Tooltip("Decides if you're comparing bits. Only cares about equalling, since it's just 0 and 1.")] private bool bitWise;
        [SerializeField, Tooltip("Does a left shift operation. 0 would be the 1 bit, 1 would be 2 bit, 2 would be 4 bit, etc.")] private byte bitToCheck;

        public bool ConditionMet()
        {
            bool met = false;
            int foundVal = Script_Manager.SM.GetFlag(flagToCheck);
            if (!bitWise)
            {
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
                    case ConditionalOperator.NotEqual:
                        if (foundVal != desiredValue) met = true;
                        break;
                }
            }
            else
            {
                int mask = 1 << bitToCheck;
                Debug.Log("Bitwise mask & foundvalue: " + (foundVal & mask));
                switch (operation)
                {
                    case ConditionalOperator.Equals:
                        if ((foundVal & mask) == 1) met = true;
                        break;
                    case ConditionalOperator.NotEqual:
                        if ((foundVal & mask) == 0) met = true;
                        break;
                    default:
                        Debug.LogWarning("Only Equals and NotEqual operations are supported for bitwise flag conditions");
                        break;

                }
            }

            return met;
        }
    }
}
