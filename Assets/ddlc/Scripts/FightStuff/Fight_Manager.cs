
using EXPERIMENTAL;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using RenCSharp.Combat.Enemies;
using RenCSharp.Combat.Interfaces;
using RenCSharp.Combat.Player;
namespace RenCSharp.Combat
{
    public sealed class Fight_Manager : MonoBehaviour
    {
        public static Fight_Manager FM;
        [SerializeField] private GameObject combatCanvas;
        [SerializeField] private Player_Object playerPrefab;
        [SerializeField] private Simple_Scene_Loader ssl;
        [SerializeField] private Player_Action_Handler pah;
        [Header("Holders")]
        [SerializeField] private Transform enemyHolder;
        [SerializeField] private Transform playerHolder;
        [SerializeField] private GameObject abilityHolder;
        [SerializeField] private TextMeshProUGUI combatTextbox;
        [Header("Arena/Enemy cosmetics")]
        [SerializeField, Min(0.1f)] private float arenaSetUpTime = 1f;
        [SerializeField, Min(0.1f)] private float enemyDamageNumberForce = 5f;
        [SerializeField, Tooltip("For handling direction text is launched in."), Range(-360,360)] private float minDeg = 0;
        [SerializeField, Tooltip("For handling direction text is launched in."), Range(-360,360)] private float maxDeg = 180;
        [SerializeField] private UI_Element enemyDamageNumber;

        private int curAttackIndex;
        private int prevAttackPosRoll, dir;
        private EnemyObject curEnemy;
        private Player_Object curPlayer;

        private bool fighting, lostFight, playerTurn, singleAttack, passedScript; //probably stupid to have all of these
        private Coroutine flavorTextRoutine;
        private GameObject playerObj;
        private List<GameObject> activeProj = new();

        public bool PlayerTurn => playerTurn;
        public bool Fighting => fighting;
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
            //below two get added in bulksetup()
            Event_Bus.TryRemoveSingleObjEvent("AddAProjectile");
            Event_Bus.TryRemoveBoolEvent("EndAFight");
        }
        #region StartUp
        public void StartAFight(EnemySO eso)
        {
            Debug.Log("Starting a fight!");
            BulkSetUp();
            playerTurn = true;
            abilityHolder.SetActive(true);
            singleAttack = false;
            passedScript = false;
            Textbox_String.PauseTextbox(true);
            Event_Bus.TryFireFloatEvent("PlayerAbilityCooldown", 0);
            curEnemy = Object_Factory.SpawnObject(eso.EnemyPrefab.gameObject, "EnemyObject", enemyHolder).GetComponent<EnemyObject>();
            curEnemy.ReceiveEnemySO(eso);
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
            Event_Bus.AddBoolEvent("EndAFight", EndAFight);
            combatCanvas.SetActive(true);
            playerHolder.gameObject.SetActive(false);
            fighting = true;
            lostFight = false;
            Event_Bus.AddSingleObjEvent("AddAProjectile", AddProjectileToList);
            prevAttackPosRoll = 0;
            Textbox_String.JumpToEndOfTextbox = false;
            curAttackIndex = 0;
            dir = 1;
            pah.StartFight();
            playerObj = Object_Factory.SpawnObject(playerPrefab.gameObject, "PlayerObject", playerHolder);
            curPlayer = playerObj.GetComponent<Player_Object>();
            curPlayer.StartOfFight();
            Event_Bus.TryFireFloatEvent("PlayerHealth", (float)Flag_Manager.GetFlag("PlayerCurHealth"));
            Event_Bus.TryFireFloatEvent("PlayerHealthPerc", (float)Flag_Manager.GetFlag("PlayerCurHealth") / (float)Flag_Manager.GetFlag("PlayerMaxHealth"));
            playerObj.SetActive(false);
        }
        #endregion

        #region FightHandling
        public void EndAFight(bool loss)
        {
            Debug.Log("Ending a fight!");
            fighting = false;
            lostFight = loss;
            Event_Bus.TryRemoveBoolEvent("EndAFight");
            Event_Bus.TryRemoveSingleObjEvent("AddAProjectile");
            pah.EndFight();
            abilityHolder.SetActive(false);
        }

        private IEnumerator RunThroughEnemy()
        {
            Textbox_String.PauseTextbox(false);
            yield return PlayerTurnRoutine(); //start off with a player turn
            while (fighting)
            {
                if (WithinScriptedAttacks())
                {
                    yield return RunThroughAttack(curEnemy.MySO.ScriptedAttacks[curAttackIndex]);
                }
                else
                {
                    yield return RunThroughAttack(curEnemy.MySO.RandomAttacks[curAttackIndex]);
                }
            }
            if (flavorTextRoutine != null) StopCoroutine(flavorTextRoutine);
            if (!lostFight) //if we won that there battle
            {
                Flag_Manager.SetFlag("PlayerCurHealth", Mathf.CeilToInt(curPlayer.CurrentHealth)); //we remember damage taken, for immersion
                Event_Bus.TryFireVoidEvent("WonFight");
                //sequence transition can optionally undo tf out of this shit.
                Object_Factory.RemoveObject("EnemyObject"); //despawn anemone
                yield return Textbox_String.RunThroughText(combatTextbox, curEnemy.MySO.DefeatText);
                yield return new WaitForSeconds(2);
                Object_Factory.RemoveObject("PlayerObject");
                Object_Factory.RemoveObject("PlayerAttack"); //do not tolerate garbage hoarding
                Event_Bus.TryFireVoidEvent("UnpauseSequence"); //allow sequence to resume
                combatCanvas.SetActive(false);
            }
            else //L Bozo
            {
                yield return LostTheFight();
            }
        }
        private IEnumerator RunThroughAttack(EnemyAttack ea)
        {
            float t = 0; //timer for the attack
            float f = ea.SecondsPerProjectileSpawn; //timer for individual projectiles
            Textbox_String.JumpToEndOfTextbox = true;
            prevAttackPosRoll = -1; //make sure that 0 is always the first for an attack?
            yield return SetUpArena(ea);
            playerObj.transform.localPosition = Vector3.zero; //PLEASE PLEASE DON'T BE OUTSIDE OF THE BOX DAMN YOU
            yield return new WaitForSeconds(0.75f); //wait less than a second before immediately spawning an projectile

            while (t <= ea.AttackDuration && fighting)
            {
                t += Time.deltaTime;
                f += Time.deltaTime; //screw it, second timer for spawning projectiles
                if (f >= ea.SecondsPerProjectileSpawn)
                {
                    f = 0;
                    for (int i = 0; i < ea.ProjectilesPerSpawn; i++) //spawn as many projectiles as we want in one go. the intelligent rolling should prevent bs
                    {
                        //roll which position/direction we have when first spawning a projectile
                        if (prevAttackPosRoll == 0) dir = 1;
                        else if (prevAttackPosRoll >= ea.SpawnPoints.Length - 1) dir = -1;

                        int randI = ea.ProjectileSpawnPositionMethod switch
                        {
                            AttackSpawnSelectionMethod.TrueRandom => Random.Range(0, ea.SpawnPoints.Length),
                            AttackSpawnSelectionMethod.NoRepeatRandom => RandomHelper.NoRepeatRoll("attackSpawnRoll", ea.SpawnPoints.Length),
                            AttackSpawnSelectionMethod.LoopThrough => (prevAttackPosRoll >= ea.SpawnPoints.Length - 1) ? 0 : prevAttackPosRoll + 1,
                            AttackSpawnSelectionMethod.ReverseLoopThrough => (prevAttackPosRoll <= 0) ? ea.SpawnPoints.Length - 1 : prevAttackPosRoll - 1,
                            AttackSpawnSelectionMethod.PingPong => prevAttackPosRoll += dir,
                            _ => 0 //default scenario of garbage null enum, just return 0 and probably complain too
                        };

                        if (randI >= ea.Indexes.Length) randI = 0; //panic grab 0 index if we suck nuts
                        prevAttackPosRoll = randI;

                        Base_Projectile projToSpawn = ea.ProjectilesThatSpawn[ea.Indexes[randI]];
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
                }

                yield return null;
            }
            playerTurn = true;
            for (int i = activeProj.Count - 1; i >= 0; i--) //despawn all projectiles after attack is done
            {
                if (activeProj[i].activeInHierarchy) Object_Pooling.Despawn(activeProj[i], true);
                activeProj.RemoveAt(i);
            }
            ea.ControlType.ExitControl(); //disable player dodge object and turn off its controls
            playerObj.SetActive(false);
            if (!singleAttack)
            {
                curAttackIndex++;
                if(!WithinScriptedAttacks()) 
                {
                    passedScript = true;
                    curAttackIndex = RandomHelper.NoRepeatRoll("RandomAttackID", curEnemy.MySO.RandomAttacks.Length); 
                }
            }

            yield return CloseArena();

            if(!singleAttack) yield return PlayerTurnRoutine();
            else if(!lostFight)
            {
                Event_Bus.TryFireVoidEvent("WonFight");
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
        /// <summary>
        /// Adds a gameobject to the projectile list, so we can clean them up when the attack is finished.
        /// </summary>
        /// <param name="obj">GAMEOBJEKT the gameobject we are adding to list</param>
        private void AddProjectileToList(object obj)
        {
            GameObject go = (GameObject) obj;
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
        #endregion

        #region Arena
        private IEnumerator SetUpArena(EnemyAttack ea)
        {
            float t = 0;
            float eval;
            playerHolder.gameObject.SetActive(true);
            RectTransform rt = playerHolder.GetComponent<RectTransform>();
            while (t <= arenaSetUpTime) //set the size of arena before spawning player back in
            {
                t += Time.deltaTime;
                eval = t / arenaSetUpTime;
                rt.sizeDelta = Vector2.Lerp(Vector2.zero, ea.ArenaDimensions, eval);
                yield return null;
            }
            playerObj.SetActive(true);
            playerObj.transform.SetParent(playerHolder); //guarantee????
            playerObj.transform.localPosition = Vector3.zero; //reset to origin of holder?
            //if (curAttackIndex == 0 && !passedScript) playerObj.GetComponent<Player_Object>().StartOfFight();
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
        #endregion

        #region Player
        private IEnumerator PlayerTurnRoutine()
        {
            Event_Bus.TryFireDoubleObjEvent("SetPlayerResistance", (object)true, (object)0f); //reset player resistance to value not affected by defend
            if (flavorTextRoutine != null) StopCoroutine(flavorTextRoutine);
            if (!WithinScriptedAttacks() && curAttackIndex >= curEnemy.MySO.RandomAttacks.Length) //evil ah check that's probably because
                                                                                                  //of a random timing problem somewhere
            {
                curAttackIndex = RandomHelper.NoRepeatRoll("RandomAttackID", curEnemy.MySO.RandomAttacks.Length);
                Debug.Log("New Rolled curAttackIndex: " + curAttackIndex);
            }
            flavorTextRoutine = StartCoroutine(Textbox_String.RunThroughText(combatTextbox, WithinScriptedAttacks() ? 
                curEnemy.MySO.ScriptedFlavorTexts[curAttackIndex] : curEnemy.MySO.RandomFlavorTexts[curAttackIndex]));

            pah.StartPlayerTurn();

            while (playerTurn && fighting)
            {
                if(!pah.PlayerActionLockedIn) yield return null; //idle until the player locks in an action
                else
                {
                    yield return pah.CurrentPlayerAction.ActionResult();
                    pah.EndPlayerTurn();
                    playerTurn = false;
                }
            }
        }
        #endregion
        void SpawnEnemyDamageNumber(float damageTaken)
        {
            UI_Element fella = Object_Pooling.Spawn(enemyDamageNumber.gameObject, curEnemy.transform.position, Quaternion.identity).GetComponent<UI_Element>();
            fella.transform.SetParent(curEnemy.transform); //'cause canvas chicanery mostly
            StartCoroutine(Object_Pooling.DespawnOverTime(fella.gameObject, 2f));
            fella.Texts[0].text = "-" + damageTaken.ToString("n1");
            Vector3 lauchDir = Noise_Helper.SineNoiseVector(Mathf.Deg2Rad * minDeg, Mathf.Deg2Rad * maxDeg);
            lauchDir.Set(lauchDir.x, lauchDir.y, 0);
            Rigidbody rb = fella.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(lauchDir * enemyDamageNumberForce, ForceMode.VelocityChange);
        }

        private bool WithinScriptedAttacks()
        {
            if (curAttackIndex < curEnemy.MySO.ScriptedAttacks.Length && !passedScript)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
