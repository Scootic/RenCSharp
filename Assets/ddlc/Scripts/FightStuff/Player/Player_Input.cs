using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
namespace RenCSharp.Combat
{
    public class Player_Input : MonoBehaviour
    {
        [SerializeField] private InputActionReference movement, attack;

        public static Action<Vector2> Movement;
        public static Action Attack;

        private bool move;

        private void OnEnable()
        {
            movement.action.started += ctx => StartCoroutine(ReadMovement()); 
            movement.action.canceled += ctx => move = false; 
            attack.action.performed += ctx => Attack?.Invoke();

            movement.action.Enable();
            attack.action.Enable();

            //Movement += DebugInputMove;
        }

        private IEnumerator ReadMovement()
        {
            move = true;
            while (move)
            {
                Movement?.Invoke(movement.action.ReadValue<Vector2>());
                yield return null;
            }
            Movement?.Invoke(Vector2.zero);
        }

        private void DebugInputMove(Vector2 v2)
        {
            Debug.Log("Input Grabbed: " + v2);
        }

        private void OnDisable()
        {
            movement.action.started -= ctx => StartCoroutine(ReadMovement());
            movement.action.canceled -= ctx => move = false;
            attack.action.performed -= ctx => Attack?.Invoke();

            movement.action.Disable();
            attack.action.Disable();

            //Movement -= DebugInputMove;
        }
    }
}
