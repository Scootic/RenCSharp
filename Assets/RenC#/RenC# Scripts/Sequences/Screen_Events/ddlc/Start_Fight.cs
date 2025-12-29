using UnityEngine;
using RenCSharp.Combat;
namespace RenCSharp.Sequences
{
    public class Start_Fight : Screen_Event
    {
        [SerializeField] private EnemySO enemyToLoad;
        public override void DoShit()
        {
            Fight_Manager.FM.StartAFight(enemyToLoad);
        }

        public override string ToString()
        {
            return "Start a Fight";
        }
    }
}
