using UnityEngine;

namespace RenCSharp.Menus
{
    public class History_Menu : Menu_Base
    {
        [SerializeField] private Transform historyHolder;
        [SerializeField] private GameObject historyPrefab;
        [SerializeField] private GameObject historyMenu;
        [SerializeField] private float expectedHistoryObjectHeight = 200;
        int activeHistories = 0;
        private Awaitable spawner;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override async Awaitable OnMenuOpen()
        {
            historyMenu.SetActive(true);
            History t = Script_Manager.SM.CurrentHistory;
            RectTransform rt = historyHolder.GetComponent<RectTransform>();
            Debug.Log("History lengther: " + t.DialogBoxes.Length);
            for (int i = 0; i < t.HistoryLength; i++)
            {
                if (t.SpeakerNames[i] == null) continue;
                spawner = SpawnHistory(i, t.SpeakerNames[i], t.DialogBoxes[i]);
                await spawner;
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, activeHistories * expectedHistoryObjectHeight);
                await Awaitable.NextFrameAsync();
            }
        }

        private async Awaitable SpawnHistory(int i, string speaker, string dialoge)
        {
            UI_Element uie = Object_Factory.SpawnObject(historyPrefab, "History" + i, historyHolder).GetComponent<UI_Element>();
            await Awaitable.EndOfFrameAsync();
            uie.Texts[0].text = speaker;
            uie.Texts[1].text = dialoge;
            activeHistories++;
        }

        // Update is called once per frame
        public override void OnMenuClose()
        {
            //if (!spawner.IsCompleted) spawner.Cancel();
            for (int i = activeHistories - 1; i >= 0; i--)
            {
                Object_Factory.RemoveObject("History" + i);
            }
            activeHistories = 0;
            historyMenu.SetActive(false);
        }
    }
}
