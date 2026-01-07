using System;
using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Yuri_Knives : Player_Ability
    {
        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            Debug.LogError("YURI KNIVES Not Yet Implemented!");
        }
    }
}
