using UnityEngine;
using RenCSharp.Combat;
using RenCSharp.Combat.Enemies;
namespace RenCSharp.Sequences
{
    public class Start_Fight : Screen_Event
    {
        [SerializeField] private EnemySO enemyToLoad;
        [SerializeField] private string autoSaveName = "!AutoSave_Fight";

        public override async void DoEvent()
        {
            await Script_Manager.SM.SaveGameAsync(autoSaveName, true);
            Fight_Manager.FM.StartAFight(enemyToLoad);
        }

        public override string ToString()
        {
            return "Fight/" +
                "Start a Fight";
        }
    }
}
