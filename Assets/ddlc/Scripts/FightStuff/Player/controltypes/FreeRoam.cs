using UnityEngine;

namespace RenCSharp.Combat
{
    public class FreeRoam : ControlType
    {
        [SerializeField, Min(0.1f)] private float speed = 5;
        protected override void MovementEffect(Vector2 dir)
        {
            Vector3 newV = new Vector3(dir.x, dir.y, 0);
            playerObj.transform.position += newV * Time.deltaTime * speed;
        }
    }
}
