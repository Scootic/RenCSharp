using RenCSharp.EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Menus
{
    public class History_Menu : Menu_Base
    {
        [SerializeField] private Transform historyHolder;
        [SerializeField] private GameObject historyPrefab;
        [SerializeField] private GameObject historyMenu;
        [SerializeField, Tooltip("Expected Y size of history prefab. Not necessarily equal to prefab.rectTransform.height")] private float expectedHistoryObjectHeight = 200;
        int activeHistories = 0;
        private Awaitable spawner;
        private static History curHistory;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void GetHistoryBus()
        {
            //don't need to worry about removing this, Application.quitting removes ALL Event_Bus events.
            Event_Bus.AddSingleObjEvent("HistoryUpdated", ReceiveHistory);
        }

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

        private static void ReceiveHistory(object HISTORY)
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
