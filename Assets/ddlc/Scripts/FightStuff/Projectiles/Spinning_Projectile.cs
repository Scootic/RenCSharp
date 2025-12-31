using UnityEngine;

namespace RenCSharp.Combat
{
    public class Spinning_Projectile : Base_Projectile
    {
        [Header("Spinnering")]
        [SerializeField] private float rotationSpeed = 1;

        // Update is called once per frame
        override protected void Update()
        {
            base.Update();
            Vector3 eulers = transform.rotation.eulerAngles;
            eulers.Set(0, 0, eulers.z + Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.Euler(eulers);
        }
    }
}
