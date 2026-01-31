using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Spawn_Poem : Screen_Event
    {
        [SerializeField] private Poem poemToSpawn;
        [SerializeField] private GameObject poemGOFab;
        private bool here;
        public override void DoShit()
        {
            here = false;
            SpawnGuy();
            Script_Manager.ProgressScreenEvent += SpawnGuy;
        }

        private void SpawnGuy()
        {
            if (here) { Debug.Log("Poem's already spawnt!"); return; }
            Debug.Log("Spawning a Poem!");
            GameObject poemGO = Object_Factory.SpawnObject(poemGOFab, "Poem", GameObject.Find("Foregroundcanvas").transform);
            //pass in values.
            if (poemGO == null) return;
            UI_Element uie = poemGO.GetComponent<UI_Element>();
            uie.Texts[0].text = poemToSpawn.PoemText;
            uie.Texts[0].font = poemToSpawn.PoemFont;
            here = true;
        }

        public override string ToString()
        {
            return "Spawn Poem";
        }
    }
}
