using RenCSharp.Combat;
using UnityEngine;
using RenCSharp.Combat.Enemies;
namespace RenCSharp.Sequences
{
    public class Start_Single_Attack : Screen_Event
    {
        [SerializeField] private EnemyAttack attackToRunThrough;
        public override void DoEvent()
        {
            Fight_Manager.FM.StartASingleAttack(attackToRunThrough);
        }

        public override string ToString()
        {
            return "Fight/" +
                "Run Through Single Enemy Attack";
        }
    }
}
