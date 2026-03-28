using System.Collections;
using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Platformer : ControlType
    {
        [SerializeField, Min(0.1f)] private float speed = 500, gravityForce = 600;
        [Header("Jumping")]
        [SerializeField] private float jumpForce = 500;
        [SerializeField] private float groundedDistance = 26f;
        [SerializeField] private float jumpcd = 0.2f;
        [SerializeField, Tooltip("Set this bastard to be PlayerWall bcuz Unity is a stinker 9000")] private LayerMask jumpMask;
        private bool incd = false;
        private Rigidbody rb;
        private RB_ConstForce gravity;
        private Coroutine routine;
        public override void EnterControl()
        {
            base.EnterControl();
            rb = playerObj.GetComponent<Rigidbody>();
            incd = false;
            gravity = playerObj.gameObject.AddComponent<RB_ConstForce>();
            gravity.InitForce(Vector3.down, gravityForce, true);
        }

        protected override void MovementEffect(Vector2 dir)
        {
            if(Grounded() && dir.y > 0.5f)
            {
                //Debug.Log("Player jumped");
                incd = true;
                routine = playerObj.StartCoroutine(Jumpcd());
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange); //should update linvelo before we do below...?
            }

            Vector3 newV = new Vector3(dir.x * speed, rb.linearVelocity.y, 0);
            rb.linearVelocity = newV;
        }

        public override void ExitControl()
        {
            rb.linearVelocity = Vector3.zero;
            base.ExitControl();
            GameObject.Destroy(gravity);
            if (routine != null) playerObj.StopCoroutine(routine);
        }

        private IEnumerator Jumpcd()
        {
            yield return new WaitForSeconds(jumpcd);
            incd = false;
        }

        private bool Grounded()
        {
            if (incd) return false;

            if(Physics.Raycast(playerObj.transform.position, Vector3.down, out RaycastHit shit, groundedDistance, jumpMask)) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override Color PlayerColor()
        {
            return Color.blue;
        }

        public override string ToString()
        {
            return "Platformer";
        }
    }
}
