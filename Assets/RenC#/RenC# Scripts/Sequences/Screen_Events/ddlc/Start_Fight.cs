using UnityEngine;
using RenCSharp.Combat;
using RenCSharp.Combat.Enemies;
namespace RenCSharp.Sequences
{
    public class Start_Fight : Screen_Event
    {
        [SerializeField] private EnemySO enemyToLoad;
        public override void DoEvent()
        {
            Fight_Manager.FM.StartAFight(enemyToLoad);
        }

        public override string ToString()
        {
            return "Fight/" +
                "Start a Fight";
        }
    }
}
