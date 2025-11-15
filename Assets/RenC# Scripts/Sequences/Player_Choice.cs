using UnityEngine;
using System;
namespace RenCSharp.Sequences
{
    [Serializable]
    public struct Player_Choice
    {
        [SerializeField] private string choiceText;
        [SerializeField] private Sequence resultingSequence;

        public Sequence ResultingSequence => resultingSequence;
        public string ChoiceText => choiceText;
    }
}
