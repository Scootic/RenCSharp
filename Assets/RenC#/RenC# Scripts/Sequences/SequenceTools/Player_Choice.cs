using UnityEngine;
using System;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Options that the player can choose between at the end of a sequence using buttons spawned by SM.
    /// These choices can have conditions based on flags, but these conditions are optional.
    /// </summary>
    [Serializable]
    public struct Player_Choice
    {
        [SerializeField] private string choiceText;
        [SerializeField] private Sequence resultingSequence;
        [Header("Conditions")]
        [SerializeField] private bool requireCondition;
        [SerializeField] private FlagCondition[] conditions;

        public Sequence ResultingSequence => resultingSequence;
        public string ChoiceText => choiceText;
        public bool RequireCondition => requireCondition;
        public bool MetAllConditions()
        {
            bool met = true;

            foreach(FlagCondition cond in conditions)
            {
                if (!cond.ConditionMet()) { met = false; break; } 
            }

            return met;
        }
    }
}
