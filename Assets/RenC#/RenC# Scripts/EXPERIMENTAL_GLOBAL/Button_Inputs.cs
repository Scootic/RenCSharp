using UnityEngine;
using UnityEngine.InputSystem;
using System;
namespace RenCSharp.EXPERIMENTAL
{
    public sealed class Button_Inputs : MonoBehaviour
    {
        [SerializeField] private InputActionReference progressTextBox, openConsole;

        public static Action ProgressTextBox, OpenConsole;

        private void OnEnable()
        {
            progressTextBox.action.performed += ctx => ProgressTextBox?.Invoke();
            progressTextBox.action.Enable();

            openConsole.action.performed += ctx => OpenConsole?.Invoke();
            openConsole.action.Enable();
        }

        private void OnDisable()
        {
            ProgressTextBox = null;
            OpenConsole = null;

            progressTextBox.action.performed -= ctx => ProgressTextBox?.Invoke();
            progressTextBox.action.Disable();

            openConsole.action.performed -= ctx => OpenConsole?.Invoke();
            openConsole.action.Disable();
        }
    }
}
