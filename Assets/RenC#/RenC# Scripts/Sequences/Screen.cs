using RenCSharp.Actors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// A subdivision of sequences. Essentially, one screen should equal one text box of dialog. They contain a reference to an actor
    /// (who will be the name that appears over the dialog box), the dialog itself, and a list of Screen Events that are fired by the 
    /// Script_Manager when the screen becomes current. If there's no actor, it will be considered as narration. Screen Events still fire
    /// even if there's no dialog. This can be used for doing things without any dialog, or as an empty after certain dialog.
    /// </summary>
    [Serializable]
    public class Screen
    {
        [SerializeField] private Actor speaker = null;
        [SerializeField, TextArea(3, 7)] private string dialog = "";
        [SerializeReference] public List<Screen_Event> ScreenActions = new();
        public Actor Speaker => speaker;
        public string Dialog => dialog;

        public Actor SetSpeaker { set { speaker = value; } }
        public string SetDialog { set { dialog = value; } }
    }
}
