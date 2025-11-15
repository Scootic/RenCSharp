using RenCSharp.Actors;
using System;
using UnityEngine;
using System.Collections.Generic;
namespace RenCSharp.Sequences
{
    [Serializable]
    public class Screen 
    {
        [SerializeField] private Actor speaker;
        [SerializeField, TextArea] private string dialog;
        [SerializeReference] public List<Screen_Event> ScreenActions;
        public Actor Speaker => speaker;
        public string Dialog => dialog;

    }
}
