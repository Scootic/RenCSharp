using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class StraightLine_Projectile_MovementType : Projectile_MovementType
    {
        public override void MovementBehavior()
        {
            projectileTransform.position += speed * Time.deltaTime * moveDir;
        }
        public override Vector2 GetPositionAtTime(float time, Vector2 initialDirection, Vector3 spawnPos)
        {
            Vector2 s = new Vector2(spawnPos.x, spawnPos.y);
            return s + (speed * time) * initialDirection;
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
