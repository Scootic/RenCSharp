
using UnityEngine;
using EXPERIMENTAL;
using System.Collections;
namespace RenCSharp.Combat
{
    public class Fight_Manager : MonoBehaviour
    {
        public static Fight_Manager FM;
        [SerializeField] private EnemyObject enemyPrefab;
        [SerializeField] private Transform enemyHolder;

        private int curAttackIndex;
        private EnemyObject curEnemy;
        private bool fighting;

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
                yield return null;
            }

            //do an end of battle animation thing
        }

        private IEnumerator RunThroughAttack(EnemyAttack ea)
        {
            yield return null;
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
