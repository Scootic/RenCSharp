using UnityEngine;

namespace RenCSharp.Sequences
{
    public class AutoSave_Screen : Screen_Event
    {
        [SerializeField] private string fileName = "AutoSave";
        public override void DoEvent()
        {
            Debug.Log("Autosaving game!");
            Sequence_Manager.ProgressScreenEvent += PanicStop;
        }

        private void PanicStop()
        {
            Sequence_Manager.SM.SaveGameData(fileName, true);
            Sequence_Manager.ProgressScreenEvent -= PanicStop;
        }

        public override string ToString()
        {
            return "Auto Save";
        }
        
    }
}
