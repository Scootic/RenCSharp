using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace RenCSharp
{
    /// <summary>
    /// class for spawning and handling gameobjects. mostly to use gameobject.find() less.
    /// </summary>
    public static class Object_Factory 
    {
        private static Dictionary<string, GameObject> activeGameObjects = new Dictionary<string, GameObject>();

        public static GameObject GetObject(string name)
        {
            if (activeGameObjects.ContainsKey(name)) return activeGameObjects[name];
            else
            {
                Debug.LogWarning("No active object of name: " + name);
                return null;
            }
        }

        public static GameObject SpawnObject(GameObject prefab, string name, Transform parent = null) 
        {
            GameObject t = GameObject.Instantiate(prefab, parent);
            t.name = name;
            activeGameObjects.Add(name, t);
            return t;
        }

        public static void RemoveObject(string name)
        {
            if (activeGameObjects.ContainsKey(name))
            {
                GameObject t = activeGameObjects[name];
                activeGameObjects.Remove(name);
                GameObject.Destroy(t);
            } Debug.LogWarning("No active gameobject of name: " + name);
        }
        /// <summary>
        /// Very dangerous. Only should be used OnDisable for SM or sum shit;
        /// </summary>
        public static void ScrubDictionary()
        {
            activeGameObjects = new Dictionary<string, GameObject>();
        }
    }
}
