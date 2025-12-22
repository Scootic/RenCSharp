using UnityEngine;

namespace RenCSharp
{
    public class Persistent_Flags_On_Awake : MonoBehaviour
    {
        private void Awake()
        {
            FlagToken ft = SaveLoad.LoadPersistentFlags();
            Flag_Manager.ReceiveFlagToken(ft, true);
        }
    }
}
