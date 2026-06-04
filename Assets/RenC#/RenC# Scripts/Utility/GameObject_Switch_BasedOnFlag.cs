using System;
using UnityEngine;

namespace RenCSharp
{
    public sealed class GameObject_Switch_BasedOnFlag : MonoBehaviour
    {
        [SerializeField, Tooltip("If true, toggles GOs in Awake, else toggle GOs in Start")] private bool fireOnAwake = true;
        [SerializeField] private FlagConditionalGameObject[] conditionalGameObjects;

        void Awake()
        {
            if (fireOnAwake) RunThroughGameObjectList();
        }

        void Start()
        {
            if (!fireOnAwake) RunThroughGameObjectList();
        }

        void RunThroughGameObjectList()
        {
            if(conditionalGameObjects.Length == 0)
            {
                Debug.LogWarning($"{gameObject.name}'s Flag-based GameObject Switch has no elements!");
                return;
            }
            foreach(FlagConditionalGameObject fcgo in conditionalGameObjects) 
            {
                fcgo.go.SetActive(fcgo.conditional.ConditionMet());
            }
        }

        [Serializable]
        private struct FlagConditionalGameObject
        {
            public FlagCondition conditional;
            public GameObject go;
        }
    }
}
