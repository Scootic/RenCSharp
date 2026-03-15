using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class StraightLine_Projectile_MovementType : Projectile_MovementType
    {
        public override void MovementBehavior()
        {
            projectileTransform.position += moveDir * speed * Time.deltaTime;
        }
        public override string ToString()
        {
            return "Straight Line";
        }
        public override void OnEditorValidate()
        {
            return;
        }
    }
}
