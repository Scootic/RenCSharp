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
        [SerializeField] private FlagCondition[] conditions;
        public Sequence ResultingSequence => resultingSequence;
        public string ChoiceText => choiceText;
        public FlagCondition[] Conditions => conditions;

        public Sequence SetResultingSequence { set {  resultingSequence = value; } }
        public string SetChoiceText { set { choiceText = value; } }
        public FlagCondition[] SetConditions { set  { conditions = value; } }

        public bool MetAllConditions()
        {
            if (conditions.Length == 0) return true;
            bool met = true;

            foreach(FlagCondition cond in conditions)
            {
                if (!cond.ConditionMet()) { met = false; break; } 
            }

            return met;
        }
    }
}
