
using EXPERIMENTAL;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

namespace RenCSharp.Combat
{
    public sealed class Fight_Manager : MonoBehaviour
    {
        public static Fight_Manager FM;
        [SerializeField] private GameObject combatCanvas;
        [SerializeField] private EnemyObject enemyPrefab;
        [SerializeField] private Player_Object playerPrefab;
        [SerializeField] private Simple_Scene_Loader ssl;
        [Header("Holders")]
        [SerializeField] private Transform enemyHolder;
        [SerializeField] private Transform playerHolder;
        [SerializeField] private TextMeshProUGUI combatTextbox;
        [Header("Arena")]
        [SerializeField, Min(0.1f)] private float arenaSetUpTime = 1f;
        [SerializeField, Min(0.1f)] private float enemyDamageNumberForce = 5f;
        [SerializeField, Tooltip("For handling direction text is launched in."), Range(-360,360)] private float minDeg = 0;
        [SerializeField, Tooltip("For handling direction text is launched in."), Range(-360,360)] private float maxDeg = 180;
        [SerializeField] private UI_Element enemyDamageNumber;
        [Header("PlayerAttack")]
        [SerializeField] private float playerAttackAnimationDuration;
        [SerializeField, Range(0, 1)] private float playerAttackVolMult = 0.5f;
        [SerializeField] private Sprite[] playerAttackAnimFrames;
        [SerializeField] private UI_Element playerAttackFab;
        [SerializeField] private AudioSource attackSound;

        private int curAttackIndex;
        private int prevAttackRoll, dir;
        private EnemyObject curEnemy;
        private Player_Object curPlayer;
        private UI_Element curPAttack;
        private bool fighting, lostFight, playerTurn, playerAttack, singleAttack;
        private Coroutine flavorTextRoutine;
        private GameObject playerObj;
        private List<GameObject> activeProj = new();

        public bool PlayerTurn => playerTurn;

        private void Awake()
        {
            if (FM == null) FM = this;
            else if (FM != this) { Destroy(FM); FM = this; }
        }

        private void OnEnable()
        {
            Event_Bus.AddFloatEvent("EnemyDamageNumber", SpawnEnemyDamageNumber);
        }

        private void OnDisable()
        {
            Event_Bus.TryRemoveFloatEvent("EnemyDamageNumber");
        }

        public void StartAFight(EnemySO eso)
        {
            Debug.Log("Starting a fight!");
            BulkSetUp();
            playerTurn = true;
            singleAttack = false;
            Textbox_String.PauseTextbox(true);
            curEnemy = Object_Factory.SpawnObject(enemyPrefab.gameObject, "EnemyObject", enemyHolder).GetComponent<EnemyObject>();
            curEnemy.ReceiveEnemySO(eso);
            Player_Input.Attack += PlayerAttack;
            StartCoroutine(RunThroughEnemy());
        }

        public void StartASingleAttack(EnemyAttack ea)
        {
            singleAttack = true;
            playerTurn = false;
            BulkSetUp();
            combatTextbox.text = "";
            StartCoroutine(RunThroughAttack(ea));
        }

        private void BulkSetUp()
        {
            Event_Bus.TryFireVoidEvent("PauseSequence");
            combatCanvas.SetActive(true);
            playerHolder.gameObject.SetActive(false);
            fighting = true;
            lostFight = false;
            playerAttack = false;
            prevAttackRoll = 0;
            Textbox_String.JumpToEndOfTextbox = false;
            curAttackIndex = 0;
            dir = 1;
            playerObj = Object_Factory.SpawnObject(playerPrefab.gameObject, "PlayerObject", playerHolder);
            curPlayer = playerObj.GetComponent<Player_Object>();
            playerObj.SetActive(false);
        }

        public void EndAFight(bool loss)
        {
            Debug.Log("Ending a fight!");
            fighting = false;
            lostFight = loss;
        }

        private IEnumerator RunThroughEnemy()
        {
            Textbox_String.PauseTextbox(false);
            flavorTextRoutine = StartCoroutine(Textbox_String.RunThroughText(combatTextbox, "Hit this fool!"));
            yield return PlayerTurnRoutine();
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
                    yield return RunThroughAttack(curEnemy.MySO.RandomAttacks[randI]);
                }
            }
            if (flavorTextRoutine != null) StopCoroutine(flavorTextRoutine);
            Player_Input.Attack -= PlayerAttack;
            if (!lostFight) //if we won that there battle
            {
                Flag_Manager.SetFlag("PlayerCurHealth", Mathf.CeilToInt(curPlayer.CurrentHealth)); //we remember damage taken, for immersion
                //sequence transition can optionally undo tf out of this shit.
                Object_Factory.RemoveObject("EnemyObject"); //despawn anemone
                yield return Textbox_String.RunThroughText(combatTextbox, curEnemy.MySO.DefeatText);
                yield return new WaitForSeconds(2);
                Object_Factory.RemoveObject("PlayerObject");
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
            Textbox_String.JumpToEndOfTextbox = true;
            yield return SetUpArena(ea);

            while (t <= ea.AttackDuration && fighting)
            {
                t += Time.deltaTime;
                f += Time.deltaTime; //screw it, second timer for spawning projectiles
                if (f >= ea.SecondsPerProjectileSpawn)
                {
                    f = 0;
                    //roll which position/direction we have when first spawning a projectile
                    if (prevAttackRoll == 0) dir = 1;
                    else if (prevAttackRoll >= ea.SpawnPoints.Length - 1) dir = -1;
                    int randI = ea.ProjectileSpawnMethod switch
                    {
                        AttackSpawnSelectionMethod.TrueRandom => Random.Range(0, ea.SpawnPoints.Length),
                        AttackSpawnSelectionMethod.NoRepeatRandom => RandomHelper.NoRepeatRoll("attackRoll", ea.SpawnPoints.Length),
                        AttackSpawnSelectionMethod.LoopThrough => (prevAttackRoll >= ea.SpawnPoints.Length - 1) ? 0 : prevAttackRoll + 1,
                        AttackSpawnSelectionMethod.PingPong => prevAttackRoll += dir,
                        _ => 0 //default scenario of garbage null enum, just return 0 and probably complain too
                    };
                    prevAttackRoll = randI;
                    //roll which projectile we shall spawn from array. (probably not as important as randspawn/dir)
                    int randI2 = Random.Range(0, ea.ProjectilesThatSpawn.Length);

                    Base_Projectile projToSpawn = ea.ProjectilesThatSpawn[randI2];
                    Vector3 spawnPosition = ea.SpawnPoints[randI];
                    Vector3 ogProjDir = ea.InitialDirections[randI];

                    Base_Projectile cur = Object_Pooling.Spawn(projToSpawn.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Base_Projectile>();
                    cur.transform.SetParent(playerHolder);
                    cur.transform.localPosition = spawnPosition;
                    cur.UpdateMoveDir(ogProjDir);
                    Vector3 soundSpawnPos = Camera.main.transform.position + cur.transform.localPosition.normalized;
                    Audio_Manager.AM.Play3DSFX(cur.SpawnSound, soundSpawnPos, false, false, cur.SpawnSoundVol, 0.9f, 1.1f);
                    AddProjectileToList(cur.gameObject);
                    StartCoroutine(Object_Pooling.DespawnOverTime(cur.gameObject, cur.Lifetime));
                }

                yield return null;
            }
            playerTurn = true;
            for (int i = activeProj.Count - 1; i >= 0; i--)
            {
                if (activeProj[i].activeInHierarchy) Object_Pooling.Despawn(activeProj[i]);
                activeProj.RemoveAt(i);
            }
            ea.ControlType.ExitControl();
            playerObj.SetActive(false);
            if (!singleAttack)
            {
                curAttackIndex++;
                if (flavorTextRoutine != null) StopCoroutine(flavorTextRoutine);
                flavorTextRoutine = StartCoroutine(Textbox_String.RunThroughText(combatTextbox, ea.PostAttackDescription));
            }
            
            playerAttack = false;
            yield return CloseArena();
            if(!singleAttack) yield return PlayerTurnRoutine();
            else if(!lostFight)
            {
                Flag_Manager.SetFlag("PlayerCurHealth", Mathf.CeilToInt(curPlayer.CurrentHealth));
                Event_Bus.TryFireVoidEvent("UnpauseSequence");
                combatCanvas.SetActive(false);
                Object_Factory.RemoveObject("PlayerObject");
            }
            else
            {
                yield return LostTheFight();
            }
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
            playerObj.transform.localPosition = Vector3.zero; //reset to origin of holder?
            if (curAttackIndex == 0) playerObj.GetComponent<Player_Object>().StartOfFight();
            ea.ControlType.EnterControl();
        }

        private IEnumerator CloseArena()
        {
            float t = arenaSetUpTime;
            float eval;
            RectTransform rt = playerHolder.GetComponent<RectTransform>();
            Vector2 startDim = rt.sizeDelta;
            while (t >= 0)
            {
                t -= Time.deltaTime;
                eval = t / arenaSetUpTime;
                rt.sizeDelta = Vector2.Lerp(Vector2.zero, startDim, eval);
                yield return null;
            }
            playerHolder.gameObject.SetActive(false);
        }

        private IEnumerator PlayerTurnRoutine()
        {
            float t = 0;
            int i = 0;
            float perc = playerAttackAnimationDuration / (float)playerAttackAnimFrames.Length;
            
            while (playerTurn && fighting)
            {
                if(!playerAttack) yield return null;
                else
                {
                    if(t == 0) //only play sounds after player input, but only one time please
                    {
                        Audio_Manager.AM.Play2DSFX(attackSound.clip, 1, 1, playerAttackVolMult, false);
                        Textbox_String.JumpToEndOfTextbox = true;
                    }
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
                                if (i == playerAttackAnimFrames.Length * 0.5f)
                                {
                                    curEnemy.TakeDamage(Flag_Manager.GetFlag("PlayerDamage", false), false);
                                }
                            }
                            else //do bs
                            {
                                float approxI = i + 0.5f;
                                if (Mathf.Approximately(approxI, playerAttackAnimFrames.Length * 0.5f))
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

        public void AddProjectileToList(GameObject go)
        {
            activeProj.Add(go);
        }

        private IEnumerator LostTheFight()
        {
            Textbox_String.PauseTextbox(false);
            playerHolder.gameObject.SetActive(false);
            yield return Textbox_String.RunThroughText(combatTextbox, "Good going idiot, you died! You're going back to the main menu now.");
            yield return new WaitForSeconds(2);
            Object_Factory.RemoveObject("EnemyObject");
            Object_Factory.RemoveObject("PlayerObject");
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

        void SpawnEnemyDamageNumber(float damageTaken)
        {
            UI_Element fella = Object_Pooling.Spawn(enemyDamageNumber.gameObject, curEnemy.transform.position, Quaternion.identity).GetComponent<UI_Element>();
            fella.transform.SetParent(curEnemy.transform);
            StartCoroutine(Object_Pooling.DespawnOverTime(fella.gameObject, 2f));
            fella.Texts[0].text = "-" + damageTaken.ToString("n1");
            Vector3 lauchDir = Noise_Helper.SineNoiseVector(Mathf.Deg2Rad * minDeg, Mathf.Deg2Rad * maxDeg);
            lauchDir.Set(lauchDir.x, lauchDir.y, 0);
            Rigidbody rb = fella.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(lauchDir * enemyDamageNumberForce, ForceMode.VelocityChange);
        }
    }
}
