using System;
using UnityEngine;

namespace RenCSharp.Sequences
{
    /// <summary>
    /// Parent class for all the things you might want to do when you hit a new screen. It's important to note that these do not inherit
    /// from either Monobehavior or ScriptableObjects, so you cannot create these in the project folder or scene hierarchy; they can only
    /// exist within the screens of a Sequence SO.
    ///
    /// Contains only a single method that every child must inherit and override, which is fired by the Script_Manager whenever it enters a new screen.
    /// For all children classes, include a ToString() override for the Sequence Editor's buttons.
    /// </summary>
    [Serializable]
    public abstract class Screen_Event
    {
        public abstract void DoShit();
    }
}
