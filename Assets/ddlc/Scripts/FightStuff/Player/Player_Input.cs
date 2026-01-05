using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
namespace RenCSharp.Combat
{
    public sealed class Player_Input : MonoBehaviour
    {
        [SerializeField] private InputActionReference movement, attack, ability;

        public static Action<Vector2> Movement;
        public static Action Attack, Ability;

        private bool move;

        private void OnEnable()
        {
            //only uses FM to do coroutine to prevent garbage objs dying with scene load (we can't reference our mono, because it dies on unload)
            movement.action.started += ctx => Fight_Manager.FM.StartCoroutine(ReadMovement()); 
            movement.action.canceled += ctx => move = false; 
            attack.action.performed += ctx => Attack?.Invoke();
            ability.action.performed += ctx => Ability?.Invoke();

            movement.action.Enable();
            attack.action.Enable();
            ability.action.Enable();
        }
        //fluh!
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
        //buh
        private void DebugInputMove(Vector2 v2)
        {
            Debug.Log("Input Grabbed: " + v2);
        }

        private void OnDisable()
        {
            Movement = null;
            Attack = null;
            Ability = null;
            Fight_Manager.FM.StopCoroutine(ReadMovement());
            movement.action.started -= ctx => Fight_Manager.FM.StartCoroutine(ReadMovement());
            movement.action.canceled -= ctx => move = false;
            attack.action.performed -= ctx => Attack?.Invoke();
            ability.action.performed -= ctx => Ability?.Invoke();

            movement.action.Disable();
            attack.action.Disable();
            ability.action.Disable();
        }
    }
}
