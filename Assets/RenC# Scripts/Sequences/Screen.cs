using RenCSharp.Actors;
using System;
using UnityEngine;
namespace RenCSharp.Sequences
{
    [Serializable]
    public class Screen
    {
        [SerializeField] private Actor speaker;
        [SerializeField, TextArea] private string dialog;
        [SerializeField] private Screen_Event[] screenActions;

        public Actor Speaker => speaker;
        public string Dialog => dialog;
        public Screen_Event[] ScreenActions => screenActions;
    }
}
