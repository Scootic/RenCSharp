using System;
using UnityEngine;
using RenCSharp.EXPERIMENTAL;
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
        [SerializeField, Tooltip("The right side of the operation.")] private int desiredValue;
        [SerializeField, Tooltip("Decides which pool of flags to check, true for persistent, false for per save file.")] private bool persistent;
        [SerializeField] private bool debugResult;
        [Header("BITWISE")]
        [SerializeField, Tooltip("Decides if you're comparing bits. Only supports the equals and notequals operators.")] private bool bitWise;
        #region Properties
        public string FlagToCheckProperty
        {
            get { return flagToCheck;  }
            set { flagToCheck = value; }
        }

        public ConditionalOperator Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        public int DesiredValue
        {
            get { return desiredValue; }
            set { desiredValue = value; }
        }

        public bool DebugResult
        {
            get { return debugResult; }
            set { debugResult = value; }
        }

        public bool Persistent
        {
            get { return persistent; }
            set { persistent = value; }
        }

        public bool BitWise
        {
            get { return  bitWise; }
            set { bitWise = value; }
        }
        #endregion
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
                        Debug.LogWarning("Only Equals and NotEqual operations are supported for bitwise flag conditions, you colossal dingleberry. Returning false.");
                        break;

                }
            }

            if (debugResult) Debug.Log($"Flag ({flagToCheck})'s value is: {foundVal}. This meets my condition? -> {met}.");

            return met;
        }
    }
}
