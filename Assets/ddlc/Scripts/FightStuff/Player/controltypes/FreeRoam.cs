using UnityEngine;

namespace RenCSharp.Combat
{
    public class FreeRoam : ControlType
    {
        [SerializeField, Min(0.1f)] private float speed = 500;
        private Rigidbody rb;
        protected override void MovementEffect(Vector2 dir)
        {
            Vector3 newV = new Vector3(dir.x, dir.y, 0) * speed;
            //Debug.Log("NEW V: " + newV);
            rb.linearVelocity = newV;
        }

        public override void EnterControl()
        {
            base.EnterControl();
            rb = playerObj.GetComponent<Rigidbody>();
        }

        protected override Color PlayerColor()
        {
            return Color.red;
        }
        public override string ToString()
        {
            return "Freeroam";
        }
    }
}
