using UnityEngine;

namespace RenCSharp.Sequences
{
    public class AutoSave_Screen : Screen_Event
    {
        [SerializeField] private string fileName = "AutoSave";
        public override void DoEvent()
        {
            Debug.Log("Autosaving game!");
            Script_Manager.ProgressScreenEvent += PanicStop;
        }

        private void PanicStop()
        {
            Script_Manager.SM.SaveGameData(fileName, true);
            Script_Manager.ProgressScreenEvent -= PanicStop;
        }

        public override string ToString()
        {
            return "Auto Save";
        }
    }
}
