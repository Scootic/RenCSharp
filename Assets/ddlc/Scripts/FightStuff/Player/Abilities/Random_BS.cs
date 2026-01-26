using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Player
{   
    public class Random_BS : Player_Ability
    {
        private string s;

        private void Start()
        {
            Event_Bus.AddStringEvent("GrabEnemyString", GrabStupidEnemyInformation);
        }

        private void OnDisable()
        {
            Event_Bus.TryRemoveStringEvent("GrabEnemyString");
        }

        public override void FireAbility()
        {
            base.FireAbility();
            if (!validToFire || PlayerTurn) return;
            validToFire = false;
            Event_Bus.TryFireStringEvent("Investigate", s);
        }

        private void GrabStupidEnemyInformation(string se)
        {
            Debug.Log("Grabbed enemy info: " + se);
            s = se;
            
        }
    }
}
