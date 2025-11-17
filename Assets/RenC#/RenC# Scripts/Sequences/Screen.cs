using RenCSharp.Actors;
using System;
using UnityEngine;
using System.Collections.Generic;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// A subdivision of sequences. Essentially, one screen equals one text box of dialog. They contain a reference to an actor
    /// (who will be the name that appears over the dialog box), the dialog itself, and a list of Screen Events that are fired by the 
    /// Script_Manager when the screen becomes current.
    /// </summary>
    [Serializable]
    public class Screen 
    {
        [SerializeField] private Actor speaker;
        [SerializeField, TextArea(3,7)] private string dialog;
        [SerializeReference] public List<Screen_Event> ScreenActions;
        public Actor Speaker => speaker;
        public string Dialog => dialog;

    }
}
