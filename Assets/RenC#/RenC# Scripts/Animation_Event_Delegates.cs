using UnityEngine;
using System;
namespace RenCSharp
{
    /// <summary>
    /// Used so that non-monobehavior scripts can reference an animation event. (Like screen fades.)
    /// </summary>
    public sealed class Animation_Event_Delegates : MonoBehaviour
    {
        [SerializeField, Tooltip("Sets how many actions the script will have. Don't set to 0. Pretty please.")] private byte animationDelegateLength = 2;
        public Action[] AnimationDelegates;

        private void Awake()
        {
            AnimationDelegates = new Action[animationDelegateLength];
        }

        public void InvokeDelegate(int index)
        {
            AnimationDelegates[index]?.Invoke();
        }

        public void WipeDelegates()
        {
            for (int i = 0; i < AnimationDelegates.Length; i++)
            {
                AnimationDelegates[i] = null; //scrub the events so that there's no repeat bullshit
            }
        }
    }
}
