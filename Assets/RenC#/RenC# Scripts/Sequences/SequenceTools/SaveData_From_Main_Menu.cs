using UnityEngine;
using UnityEngine.SceneManagement;
namespace RenCSharp.Sequences
{
    public class SaveData_From_Main_Menu : MonoBehaviour
    {
        public SaveData SD;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void OnEnable()
        {
            SceneManager.sceneLoaded += StopSM;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= StopSM;
        }
        /// <summary>
        /// Stop the sequence manager from just going from the default sequence, actually
        /// load the guy we want.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="lsm"></param>
        void StopSM(Scene s, LoadSceneMode lsm)
        {
            if (Script_Manager.SM != null)
            {
                Script_Manager.SM.LoadShit(SD);
                Debug.Log("Should be loading aan scene!");
                Object_Factory.RemoveObject(gameObject.name);
            }
        }
    }
}
