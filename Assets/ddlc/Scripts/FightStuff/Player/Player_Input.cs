using UnityEngine;
using UnityEngine.InputSystem;
using System;
namespace RenCSharp.Combat
{
    public class Player_Input : MonoBehaviour
    {
        [SerializeField] private InputActionReference movement, attack;

        public static Action<Vector2> Movement;
        public static Action Attack;

        private void OnEnable()
        {
            movement.action.performed += ctx => Movement?.Invoke(ctx.ReadValue<Vector2>());
            attack.action.performed += ctx => Attack?.Invoke();

            movement.action.Enable();
            attack.action.Enable();
        }

        private void OnDisable()
        {
            movement.action.performed -= ctx => Movement?.Invoke(ctx.ReadValue<Vector2>());
            attack.action.performed -= ctx => Attack?.Invoke();

            movement.action.Disable();
            attack.action.Disable();
        }
    }
}
