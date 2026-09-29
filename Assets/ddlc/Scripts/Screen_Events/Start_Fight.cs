using RenCSharp.Combat;
using RenCSharp.Combat.Enemies;
using RenCSharp.EXPERIMENTAL;
using UnityEngine;
namespace RenCSharp.Sequences
{
    public class Start_Fight : Screen_Event
    {
        [SerializeField] private EnemySO enemyToLoad;
        [SerializeField] private string autoSaveName = "!AutoSave_Fight";

        public override async void DoEvent()
        {
            Event_Bus.TryFireBoolEvent("PauseSequence", false);
            await Sequence_Manager.SM.SaveGameAsync(autoSaveName, true);
            Fight_Manager.FM.StartAFight(enemyToLoad);
        }

        public override string ToString()
        {
            return "Fight/" +
                "Start a Fight";
        }
    }
}
