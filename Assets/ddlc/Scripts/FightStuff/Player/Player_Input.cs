using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
namespace RenCSharp.Combat
{
    public sealed class Player_Input : MonoBehaviour
    {
        [SerializeField] private InputActionReference movement, attack;

        public static Action<Vector2> Movement;
        public static Action Attack;

        private bool move;

        private void OnEnable()
        {
            //only uses FM to do coroutine to prevent garbage objs dying with scene load (we can't reference our mono, because it dies on unload)
            movement.action.started += ctx => Fight_Manager.FM.StartCoroutine(ReadMovement()); 
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
            Movement = null;
            Attack = null;
            Fight_Manager.FM.StopCoroutine(ReadMovement());
            movement.action.started -= ctx => Fight_Manager.FM.StartCoroutine(ReadMovement());
            movement.action.canceled -= ctx => move = false;
            attack.action.performed -= ctx => Attack?.Invoke();

            movement.action.Disable();
            attack.action.Disable();

            //Movement -= DebugInputMove;
        }
    }
}
