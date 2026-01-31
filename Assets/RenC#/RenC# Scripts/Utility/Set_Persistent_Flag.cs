
using UnityEngine;
<<<<<<< HEAD

=======
using EXPERIMENTAL;
>>>>>>> ddlcfanshit
namespace RenCSharp
{
    public class Set_Persistent_Flag : MonoBehaviour
    {
        [SerializeField] private string id;
        public void SetPersistentFlag(int value)
        {
            Flag_Manager.SetFlag(id, value, true);
        }
    }
}
