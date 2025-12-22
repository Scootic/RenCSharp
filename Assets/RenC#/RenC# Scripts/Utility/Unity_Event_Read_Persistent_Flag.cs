
using UnityEngine;
using UnityEngine.Events;
namespace RenCSharp
{
    public class Unity_Event_Read_Persistent_Flag : MonoBehaviour
    {
        [SerializeField] private UnityEvent eventToFire;
        [SerializeField] private ConditionalOperator operation;
        [SerializeField] private string flagToCheck;
        [SerializeField, Tooltip("The left side of the operation")] private int desiredValue;
        [SerializeField] private bool checkOnEnable = true;
        private void OnEnable()
        {
            if (checkOnEnable) CheckPersistentFlag();
        }
        public void CheckPersistentFlag()
        {
            bool met = false;
            int flagFoundValue = Flag_Manager.GetFlag(flagToCheck);

            switch (operation)
            {
                case ConditionalOperator.Equals:
                    if (desiredValue == flagFoundValue) met = true;
                    break;
                case ConditionalOperator.NotEqual:
                    if (desiredValue != flagFoundValue) met = true;
                    break;
                case ConditionalOperator.LessThan:
                    if (desiredValue < flagFoundValue) met = true;
                    break;
                case ConditionalOperator.GreaterThan:
                    if (desiredValue > flagFoundValue) met = true;
                    break;
                case ConditionalOperator.GreaterThanEquals:
                    if (desiredValue >= flagFoundValue) met = true;
                    break;
                case ConditionalOperator.LessThanEquals:
                    if(desiredValue <= flagFoundValue) met = true;
                    break;
            }

            if (met) eventToFire?.Invoke();
        }
    }
}
