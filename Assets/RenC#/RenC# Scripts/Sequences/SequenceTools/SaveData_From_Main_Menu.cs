using UnityEngine;

namespace RenCSharp.Sequences
{
    public class SaveData_From_Main_Menu : MonoBehaviour
    {
        public SaveData SD;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        // Update is called once per frame
        void Update()
        {
            if (Script_Manager.SM != null)
            {
                Script_Manager.SM.LoadShit(SD);
                Object_Factory.RemoveObject(gameObject.name);
            }
        }
    }
}
