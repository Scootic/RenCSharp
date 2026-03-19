using RenCSharp.Combat.Interfaces;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class Rotating_Projectile_CustomUpdate : Projectile_CustomUpdate
    {
        [SerializeField, Tooltip("Set to be self?")] private Transform guyToRotate;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField, Range(0,180), Tooltip("Random angle that projectile will set to on spawn. Roll through +- angle.")] private float rotationDeviation = 0;
        public override void UpdateBehavior()
        {
            Vector3 eulers = guyToRotate.rotation.eulerAngles;
            eulers.Set(0, 0, eulers.z + Time.deltaTime * rotationSpeed);
            guyToRotate.rotation = Quaternion.Euler(eulers);
        }

        public override void OnEnable()
        {
            float rollAngle = Random.Range(-rotationDeviation, rotationDeviation);
            guyToRotate.rotation = Quaternion.Euler(new Vector3(0, 0, rollAngle));
        }

        public override void OnRemove(bool playerTurn)
        {
            return;
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
