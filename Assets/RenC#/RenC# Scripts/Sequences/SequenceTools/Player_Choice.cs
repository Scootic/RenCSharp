using UnityEngine;
using System;
namespace RenCSharp.Sequences
{
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
        public FlagCondition[] Conditions => conditions;
    }
}
