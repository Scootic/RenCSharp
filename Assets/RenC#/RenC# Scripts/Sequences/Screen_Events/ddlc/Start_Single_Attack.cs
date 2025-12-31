using RenCSharp.Combat;
using UnityEngine;
namespace RenCSharp.Sequences
{
    public class Start_Single_Attack : Screen_Event
    {
        [SerializeField] private EnemyAttack attackToRunThrough;
        public override void DoShit()
        {
            Fight_Manager.FM.StartASingleAttack(attackToRunThrough);
        }

        public override string ToString()
        {
            return "Run Through Single Enemy Attack";
        }
    }
}
