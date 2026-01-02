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
        [SerializeField, Tooltip("The found flag value is the left side of the operation.")] private ConditionalOperator operation;
        [SerializeField] private int desiredValue;
        [SerializeField] private bool persistent;
        [Header("BITWISE")]
        [SerializeField, Tooltip("Decides if you're comparing bits. Only cares about equalling.")] private bool bitWise;

        public bool ConditionMet()
        {
            bool met = false;
            int foundVal = Flag_Manager.GetFlag(flagToCheck, persistent);
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
                //Debug.Log("Bitwise flag & desiredValue: " + (foundVal & desiredValue));
                switch (operation)
                {
                    case ConditionalOperator.Equals:
                        if ((foundVal & desiredValue) == desiredValue) met = true;
                        break;
                    case ConditionalOperator.NotEqual:
                        if ((foundVal & desiredValue) != desiredValue) met = true;
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
