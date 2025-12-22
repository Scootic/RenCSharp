using UnityEngine;

namespace RenCSharp
{
    public class Set_Persistent_Flag : MonoBehaviour
    {
        public void SetPersistentFlag(string id, int value)
        {
            Flag_Manager.SetFlag(id, value, true);
        }
    }
}
