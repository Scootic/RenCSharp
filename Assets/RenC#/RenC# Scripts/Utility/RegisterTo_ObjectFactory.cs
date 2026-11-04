using UnityEngine;

namespace RenCSharp
{
    /// <summary>
    /// Exists to register GameObject that exist on scene start (or spawned by other means) to the Object_Factory.
    /// Automatically removes the object OnDisable.
    /// </summary>
    public sealed class RegisterTo_ObjectFactory : MonoBehaviour
    {
        void OnEnable()
        {
            Object_Factory.ActiveGameObjects.Add(gameObject.name, gameObject);
        }
        void OnDisable()
        {
            Object_Factory.ActiveGameObjects.Remove(gameObject.name);
        }
    }
}
