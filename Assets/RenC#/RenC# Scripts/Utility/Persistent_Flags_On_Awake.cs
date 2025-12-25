using System.Collections.Generic;
using UnityEngine;

namespace RenCSharp
{
    /// <summary>
    /// I HIGHLY recommend giving this a bad boy a higher spot on the Script Execution Order. Just in case.
    /// </summary>
    public class Persistent_Flags_On_Awake : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("PersistentFlagAwake!");
            FlagToken ft = SaveLoad.LoadPersistentFlags();
            Flag_Manager.ReceiveFlagToken(ft, true);
            foreach(KeyValuePair<string, int> kvp in Flag_Manager.GetPersistentDataFlags)
            {
                Debug.Log("PERSISTENTFLAG: " + kvp.Key + ", Value: " + kvp.Value);
            }
        }

        private void OnDisable()
        {
            FlagToken ft = new FlagToken(Flag_Manager.GetPersistentDataFlags);
            SaveLoad.SavePersistentFlags(ft);
        }
    }
}
