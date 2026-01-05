using System;
using UnityEngine;

namespace RenCSharp.Combat
{
    public class Yuri_Knives : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || Fight_Manager.FM.PlayerTurn) return;
            Debug.LogError("YURI KNIVES Not Yet Implemented!");
        }
    }
}
