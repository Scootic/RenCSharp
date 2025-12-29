
using EXPERIMENTAL;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
namespace RenCSharp.Combat
{
    public class Fight_Manager : MonoBehaviour
    {
        public static Fight_Manager FM;
        [SerializeField] private GameObject combatCanvas;
        [SerializeField] private EnemyObject enemyPrefab;
        [SerializeField] private Player_Object playerPrefab;
        [SerializeField] private Simple_Scene_Loader ssl;

        [SerializeField] private Transform enemyHolder, playerHolder;
        [SerializeField] private TextMeshProUGUI combatTextbox;
        [Header("Arena")]
        [SerializeField, Min(0.1f)] private float arenaSetUpTime = 1f;
        [Header("PlayerAttack")]
        [SerializeField] private float playerAttackAnimationDuration;
        [SerializeField] private Sprite[] playerAttackAnimFrames;
        [SerializeField] private UI_Element playerAttackFab;

        private int curAttackIndex;
        private EnemyObject curEnemy;
        private UI_Element curPAttack;
        private bool fighting, lostFight, playerTurn, playerAttack;
        private Coroutine flavorTextRoutine;
        private GameObject playerObj;
        private List<GameObject> activeProj = new();

        private void Awake()
        {
            if (FM == null) FM = this;
            else if (FM != this) Destroy(this);
        }

        public void StartAFight(EnemySO eso)
        {
            combatCanvas.SetActive(true);
            fighting = true;
            lostFight = false;
            playerTurn = true;
            playerAttack = false;
            curAttackIndex = 0;
            Event_Bus.TryFireVoidEvent("PauseSequence");
            playerObj = Object_Factory.SpawnObject(playerPrefab.gameObject, "PlayerObject", playerHolder);
            curEnemy = Object_Factory.SpawnObject(enemyPrefab.gameObject, "EnemyObject", enemyHolder).GetComponent<EnemyObject>();
            curEnemy.ReceiveEnemySO(eso);
            Player_Input.Attack += PlayerAttack;
            StartCoroutine(RunThroughEnemy());
        }

        public void EndAFight(bool loss)
        {
            fighting = false;
            lostFight = loss;
        }

        private IEnumerator RunThroughEnemy()
        {
            flavorTextRoutine = StartCoroutine(Textbox_String.RunThroughText(combatTextbox, "Hit this fool!"));
            yield return PlayerTurn();
            while (fighting)
            {
                //Debug.Log("curAttackIndex: " + curAttackIndex + ", SA Length: " + curEnemy.MySO.ScriptedAttacks.Length);
                if (curAttackIndex < curEnemy.MySO.ScriptedAttacks.Length)
                {
                    //Debug.Log("Going through a scripted attack sequence");
                    yield return RunThroughAttack(curEnemy.MySO.ScriptedAttacks[curAttackIndex]);
                }
                else
                {
                    int randI = Random.Range(0, curEnemy.MySO.RandomAttacks.Length);
                    bool l = curEnemy.MySO.RandomAttacks.Length > 1;
                    //Debug.Log("RANDY: " + randI + ", L: " + l);
                    //Debug.Log("curEnemyRA LENGTHER: " + curEnemy.MySO.RandomAttacks.Length);
                    yield return RunThroughAttack(curEnemy.MySO.RandomAttacks[l ? randI : 0]);
                }
            }
            if (flavorTextRoutine != null) StopCoroutine(flavorTextRoutine);
            Player_Input.Attack -= PlayerAttack;
            if (!lostFight)
            {
                Object_Factory.RemoveObject("EnemyObject"); //despawn anemone
                yield return Textbox_String.RunThroughText(combatTextbox, curEnemy.MySO.DefeatText);
                Event_Bus.TryFireVoidEvent("UnpauseSequence"); //allow sequence to resume
                combatCanvas.SetActive(false);
            }
            else
            {
                yield return LostTheFight();
            }
        }

        private IEnumerator RunThroughAttack(EnemyAttack ea)
        {
            float t = 0;
            float f = 0;
            Textbox_String.PauseTextbox(true);

            yield return SetUpArena(ea);

            while (t <= ea.AttackDuration)
            {
                t += Time.deltaTime;
                f += Time.deltaTime; //screw it, second timer for spawning projectiles
                if (f >= ea.SecondsPerProjectileSpawn)
                {
                    f = 0;
                    Debug.Log("Should be spawning an proj");
                    //roll which position/direction we have when first spawning a projectile
                    int randI = Random.Range(0, ea.SpawnPoints.Length);
                    //roll which projectile we shall spawn from array. (probably not as important as randspawn/dir)
                    int randI2 = Random.Range(0, ea.ProjectilesThatSpawn.Length);

                    Base_Projectile projToSpawn = ea.ProjectilesThatSpawn[randI2];
                    Vector3 spawnPosition = ea.SpawnPoints[randI];
                    Vector3 ogProjDir = ea.InitialDirections[randI];

                    Base_Projectile cur = Object_Pooling.Spawn(projToSpawn.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Base_Projectile>();
                    cur.transform.SetParent(playerHolder);
                    cur.transform.localPosition = spawnPosition;
                    cur.UpdateMoveDir(ogProjDir);
                    activeProj.Add(cur.gameObject);
                    StartCoroutine(Object_Pooling.DespawnOverTime(cur.gameObject, cur.Lifetime));
                }

                yield return null;
            }
            for (int i = activeProj.Count - 1; i >= 0; i--)
            {
                if (activeProj[i].activeInHierarchy) Object_Pooling.Despawn(activeProj[i]);
                activeProj.RemoveAt(i);
            }
            ea.ControlType.ExitControl();
            Textbox_String.PauseTextbox(false);
            playerObj.SetActive(false);
            curAttackIndex++;
            if (flavorTextRoutine != null) StopCoroutine(flavorTextRoutine);
            flavorTextRoutine = StartCoroutine(Textbox_String.RunThroughText(combatTextbox, ea.PostAttackDescription));
            playerTurn = true;
            playerAttack = false;
            yield return PlayerTurn();
        }

        private IEnumerator SetUpArena(EnemyAttack ea)
        {
            float t = 0;
            float eval;
            playerHolder.gameObject.SetActive(true);
            RectTransform rt = playerHolder.GetComponent<RectTransform>();
            while (t <= arenaSetUpTime)
            {
                t += Time.deltaTime;
                eval = t / arenaSetUpTime;

                rt.sizeDelta = Vector2.Lerp(Vector2.zero, ea.ArenaDimensions, eval);
                

                yield return null;
            }
            playerObj.SetActive(true);
            ea.ControlType.EnterControl();
            playerObj.transform.localPosition = Vector3.zero; //reset to origin of holder?
        }

        private IEnumerator PlayerTurn()
        {
            playerHolder.gameObject.SetActive(false);
            float t = 0;
            int i = 0;
            float perc = playerAttackAnimationDuration / (float)playerAttackAnimFrames.Length;
            while (playerTurn)
            {
                if(!playerAttack) yield return null;
                else
                {
                    t += Time.deltaTime;
                    //do the animation
                    if(t >= perc)
                    {
                        t = 0;
                        i++;

                        if(i < playerAttackAnimFrames.Length)
                        {
                            curPAttack.Images[0].sprite = playerAttackAnimFrames[i];
                            //do midway logic
                            if (playerAttackAnimFrames.Length % 2 == 0) //if we have an even amount of anim frames
                            {
                                if (i == playerAttackAnimFrames.Length / 2)
                                {
                                    curEnemy.TakeDamage(Flag_Manager.GetFlag("PlayerDamage", false), false);
                                }
                            }
                            else //do bs
                            {
                                float approxI = i + 0.5f;
                                if (Mathf.Approximately(approxI, playerAttackAnimFrames.Length / 2))
                                {
                                    curEnemy.TakeDamage(Flag_Manager.GetFlag("PlayerDamage", false), false);
                                }
                            }
                        }
                        else
                        {
                            Object_Factory.RemoveObject("PlayerAttack");
                            playerTurn = false;
                        }
                    }
                    yield return null;
                    
                }
            }
        }

        private IEnumerator LostTheFight()
        {
            yield return Textbox_String.RunThroughText(combatTextbox, "Good going idiot, you died! You're going back to the main menu now.");
            ssl.LoadAnScene(1);
        }

        void PlayerAttack()
        {
            if (playerTurn && !playerAttack)
            {
                Debug.Log("Player Attacked!");
                curPAttack = Object_Factory.SpawnObject(playerAttackFab.gameObject, "PlayerAttack", curEnemy.transform).GetComponent<UI_Element>();
                curPAttack.Images[0].sprite = playerAttackAnimFrames[0];
                playerAttack = true;
            }
            else
            {
                Debug.LogWarning("Player's trying to attack when they shouldn't!");
            }
        }
    }
}
