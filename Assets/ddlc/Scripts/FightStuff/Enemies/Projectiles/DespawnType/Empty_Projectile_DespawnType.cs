using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Empty_Projectile_DespawnType : Projectile_DespawnType
    {
        public override void OnEditorValidate()
        {
            return;
        }
        public override void OnDespawn(bool playerTurn, Transform despawnTransform)
        {
            return;
        }
        public override string ToString()
        {
            return "Empty";
        }
    }
}
