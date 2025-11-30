using UnityEngine;
using TMPro;
namespace RenCSharp.Sequences
{
    public class Spawn_Poem : Screen_Event
    {
        [SerializeField] private Poem poemToSpawn;
        [SerializeField] private GameObject poemGOFab;
        public override void DoShit()
        {
            GameObject poemGO = Object_Factory.SpawnObject(poemGOFab, "Poem", GameObject.Find("Foregroundcanvas").transform);
            //pass in values.
            UI_Element uie = poemGO.GetComponent<UI_Element>();
            uie.Texts[0].text = poemToSpawn.PoemText;
            uie.Texts[0].font = poemToSpawn.PoemFont;
        }

        public override string ToString()
        {
            return "Spawn Poem";
        }
    }
}
