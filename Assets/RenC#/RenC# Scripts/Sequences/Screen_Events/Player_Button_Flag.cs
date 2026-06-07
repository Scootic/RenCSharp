using EXPERIMENTAL;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Player_Button_Flag : Screen_Event
    {
        [Header("Stinky Arrays")]
        [SerializeField] private string[] buttonNames;
        [SerializeField] private string[] flagsToAlter;
        [SerializeField] private int[] newFlagValues;
        [Header("Flag Alter Type")]
        [SerializeField] private bool increment;
        [SerializeField] private bool persistent;

        public override void DoEvent()
        {
            //spawn X buttons, and give them text corresponding to buttonNames
            Event_Bus.TryFireDoubleObjEvent("SpawnPlayerButtons", (object)buttonNames.Length, (object)buttonNames);

            List<Action> actions = new();
            //set up the actions to do stuff to flags when a button is chosen
            for (int i = 0; i < flagsToAlter.Length; i++)
            {
                string flagID = flagsToAlter[i];
                int newValue = newFlagValues[i];

                actions.Add(delegate 
                { 
                    if (increment) Flag_Manager.IncrementFlag(flagID, newValue, persistent); else Flag_Manager.SetFlag(flagID, newValue, persistent); 
                });
            }
            //assign the actions to the buttons accordingly
            Event_Bus.TryFireSingleObjEvent("AssignPlayerButtonBehavior", (object)actions);
        }

        public override string ToString()
        {
            return "Player Buttons Set Flag";
        }
    }
}
