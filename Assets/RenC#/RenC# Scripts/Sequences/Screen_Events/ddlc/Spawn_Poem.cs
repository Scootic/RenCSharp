using UnityEngine.TextCore;
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Spawn_Poem : Screen_Event
    {
        [SerializeField] private Poem poemToSpawn;
        [SerializeField] private GameObject poemGOFab;

        public override void DoEvent()
        {
            SpawnGuy();
            Script_Manager.ProgressScreenEvent += SpawnGuy;
        }

        private void SpawnGuy()
        {
            Debug.Log("Spawning a Poem!");
            GameObject poemGO = Object_Factory.SpawnObject(poemGOFab, "Poem", GameObject.Find("Foregroundcanvas").transform);
            //pass in values.
            if (poemGO == null) return;
            UI_Element uie = poemGO.GetComponent<UI_Element>();
            uie.Texts[0].text = poemToSpawn.PoemText;
            RectTransform rt = uie.Texts[0].transform.parent.GetComponent<RectTransform>();
            uie.Texts[0].font = poemToSpawn.PoemFont;
            uie.Texts[0].ForceMeshUpdate();
            FaceInfo face = uie.Texts[0].font.faceInfo;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, uie.Texts[0].textInfo.lineCount * (uie.Texts[0].fontSize + face.lineHeight) * 0.66f);
            //don't change the literal size of the text element, this code expects the text element to be scaling based on the container
            // (0,0) and (1,1) anchor points.
        }

        public override string ToString()
        {
            return "DDLC/Spawn Poem";
        }
    }
}
