
using UnityEngine;
using EXPERIMENTAL;
using System.Collections;
using TMPro;
namespace RenCSharp.Combat
{
    public class Fight_Manager : MonoBehaviour
    {
        public static Fight_Manager FM;
        [SerializeField] private EnemyObject enemyPrefab;
        [SerializeField] private Player_Object playerPrefab;
        [SerializeField] private Transform enemyHolder, playerHolder;
        [SerializeField] private TextMeshProUGUI combatTextbox;

        private int curAttackIndex;
        private EnemyObject curEnemy;
        private bool fighting;
        private Coroutine flavorTextRoutine;

        private void Awake()
        {
            if (FM == null) FM = this;
            else if (FM != this) Destroy(this);
        }

        public void StartAFight(EnemySO eso)
        {
            fighting = true;
            curAttackIndex = 0;
            Event_Bus.TryFireVoidEvent("PauseSequence");
            Object_Factory.SpawnObject(playerPrefab.gameObject, "PlayerObject", playerHolder);
            curEnemy = Object_Factory.SpawnObject(enemyPrefab.gameObject, "EnemyObject", enemyHolder).GetComponent<EnemyObject>();
            curEnemy.ReceiveEnemySO(eso);
            StartCoroutine(RunThroughEnemy());
        }

        public void EndAFight()
        {
            fighting = false;
            //Event_Bus.TryFireVoidEvent("UnpauseSequence");
        }

        private IEnumerator RunThroughEnemy()
        {
            while (fighting)
            {
                if (curAttackIndex < curEnemy.MySO.ScriptedAttacks.Length)
                {
                    yield return RunThroughAttack(curEnemy.MySO.ScriptedAttacks[curAttackIndex]);
                }
                else
                {
                    int randy = Random.Range(0, curEnemy.MySO.RandomAttacks.Length);
                    yield return RunThroughAttack(curEnemy.MySO.RandomAttacks[randy]);
                }
            }
            if (flavorTextRoutine != null) StopCoroutine(flavorTextRoutine);
            Object_Factory.RemoveObject("EnemyObject"); //despawn anemone
            yield return Textbox_String.RunThroughText(combatTextbox, curEnemy.MySO.DefeatText);
            Event_Bus.TryFireVoidEvent("UnpauseSequence");
            //do an end of battle animation thing
        }

        private IEnumerator RunThroughAttack(EnemyAttack ea)
        {
            float t = 0;
            ea.ControlType.EnterControl();
            Textbox_String.PauseTextbox(true);
            while (t <= ea.AttackDuration)
            {
                t += Time.deltaTime;
                yield return null;
            }
            ea.ControlType.ExitControl();
            Textbox_String.PauseTextbox(false);
            flavorTextRoutine = StartCoroutine(Textbox_String.RunThroughText(combatTextbox, ea.PostAttackDescription));
        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
