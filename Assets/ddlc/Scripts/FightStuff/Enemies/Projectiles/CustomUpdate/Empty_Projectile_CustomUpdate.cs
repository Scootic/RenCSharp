using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Empty_Projectile_CustomUpdate : Projectile_CustomUpdate
    {
        public override void UpdateBehavior()
        {
            
        }

        public override string ToString()
        {
            return "Empty";
        }
        public override void OnEditorValidate()
        {
            return;
        }
    }
}
