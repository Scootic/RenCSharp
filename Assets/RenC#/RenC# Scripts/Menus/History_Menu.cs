using RenCSharp.Sequences;
using UnityEngine;

namespace RenCSharp.Menus
{
    public class History_Menu : Menu_Base
    {
        [SerializeField] private Transform historyHolder;
        [SerializeField] private GameObject historyPrefab;
        [SerializeField] private GameObject historyMenu;
        int activeHistories = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnMenuOpen()
        {
            historyMenu.SetActive(true);
            History t = Script_Manager.SM.CurrentHistory;

            for (int i = 0; i < t.HistoryLength; i++)
            {
                if (t.SpeakerNames == null) continue;
                UI_Element uie = Object_Factory.SpawnObject(historyPrefab, "History" + i, historyHolder).GetComponent<UI_Element>();
                uie.Texts[0].text = t.SpeakerNames[i];
                uie.Texts[1].text = t.DialogBoxes[i];
                activeHistories++;
            }
        }

        // Update is called once per frame
        public override void OnMenuClose()
        {
            for (int i = activeHistories - 1; i >= 0; i--)
            {
                Object_Factory.RemoveObject("History" + i);
            }
            activeHistories = 0;
            historyMenu.SetActive(false);
        }
    }
}
