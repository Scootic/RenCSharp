using UnityEngine.TextCore;
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Spawn_Poem : Screen_Event
    {
        [SerializeField] private Poem poemToSpawn;
        [SerializeField] private GameObject poemGOFab;
        private bool here;
        public override void DoEvent()
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
            RectTransform rt = uie.Texts[0].transform.parent.GetComponent<RectTransform>();
            RectTransform rt2 = uie.Texts[0].GetComponent<RectTransform>();
            uie.Texts[0].font = poemToSpawn.PoemFont;
            uie.Texts[0].ForceMeshUpdate();
            FaceInfo face = uie.Texts[0].font.faceInfo;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, uie.Texts[0].textInfo.lineCount * uie.Texts[0].fontSize * (face.lineHeight + face.ascentLine) * 2.25f);
            rt2.localPosition = new Vector3(rt2.localPosition.x, rt.sizeDelta.y * 0.5f - 40, rt2.localPosition.z);
            here = true;
        }

        public override string ToString()
        {
            return "DDLC/Spawn Poem";
        }
    }
}
