using System;
using UnityEngine;
namespace RenCSharp.Sequences
{
    [Serializable]
    public struct ConditionalScreen
    {
        [SerializeField] private FlagCondition[] conditions;
        [SerializeField] private Screen resultingScreen;

        public Screen ResultingScreen => resultingScreen;
        public bool MetAllConditions()
        {
            foreach(FlagCondition cond in conditions)
            {
                if (!cond.ConditionMet()) return false;
            }
            return true;
        }
    }
}
