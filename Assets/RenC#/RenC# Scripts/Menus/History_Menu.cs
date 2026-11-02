using RenCSharp.EXPERIMENTAL;
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
        private History curHistory;

        private void OnEnable()
        {
            Event_Bus.AddSingleObjEvent("HistoryUpdated", ReceiveHistory);
        }

        private void OnDisable()
        {
            Event_Bus.TryRemoveSingleObjEvent("HistoryUpdated");
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override async Awaitable OnMenuOpen()
        {
            historyMenu.SetActive(true);
            RectTransform rt = historyHolder.GetComponent<RectTransform>();
            Debug.Log("History lengther: " + curHistory.DialogBoxes.Length);
            for (int i = 0; i < curHistory.HistoryLength; i++)
            {
                if (curHistory.SpeakerNames[i] == null) continue;
                spawner = SpawnHistory(i, curHistory.SpeakerNames[i], curHistory.DialogBoxes[i]);
                await spawner;
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, activeHistories * expectedHistoryObjectHeight);
                await Awaitable.NextFrameAsync();
            }
        }

        private void ReceiveHistory(object HISTORY)
        {
            curHistory = (History)HISTORY;
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
