using UnityEngine;
using UnityEngine.SceneManagement;
using RenCSharp.EXPERIMENTAL;
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
            if (Sequence_Manager.SM != null)
            {
                Sequence_Manager.SM.LoadGame(SD);
                Debug.Log("Should be loading aan scene!");
                Object_Factory.RemoveObject(gameObject.name);
            }
        }

        static void SpawnSDFMM(object SAVEDATA_sd)
        {
            SaveData sd = (SaveData)SAVEDATA_sd;
            SaveData_From_Main_Menu sdfmm = Object_Factory.SpawnComponent<SaveData_From_Main_Menu>("SL");
            sdfmm.SD = sd;
            DontDestroyOnLoad(sdfmm.gameObject);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AddEventToEB()
        {
            Event_Bus.AddSingleObjEvent("SaveDataFromMainMenu", SpawnSDFMM);
            //Application.quitting += () => { Event_Bus.TryRemoveSingleObjEvent("SaveDataFromMainMenu"); };
        }
    }
}
