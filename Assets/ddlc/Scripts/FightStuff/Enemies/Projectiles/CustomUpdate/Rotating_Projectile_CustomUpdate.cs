using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Rotating_Projectile_CustomUpdate : Projectile_CustomUpdate
    {
        [SerializeField, Tooltip("Set to be self?")] private Transform guyToRotate;
        [SerializeField] private float rotationSpeed = 1f;
        public override void UpdateBehavior()
        {
            Vector3 eulers = guyToRotate.rotation.eulerAngles;
            eulers.Set(0, 0, eulers.z + Time.deltaTime * rotationSpeed);
            guyToRotate.rotation = Quaternion.Euler(eulers);
        }
        public override string ToString()
        {
            return "Rotater!";
        }

        public override void OnEditorValidate()
        {
            return;
        }
    }
}
