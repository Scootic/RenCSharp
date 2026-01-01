using UnityEngine;

namespace RenCSharp.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class RB_ConstForce : MonoBehaviour
    {
        [SerializeField] private Vector3 dir;
        [SerializeField, Min(0.01f)] private float strength = 9.81f;
        [SerializeField] private bool ignoreMass = true;
        private Rigidbody rb;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            rb.AddForce(dir * strength, ignoreMass ? ForceMode.Acceleration : ForceMode.Force);
        }
    }
}
