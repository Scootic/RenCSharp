using System.Collections;
using UnityEngine;

namespace RenCSharp.Combat
{
    public class Player_Action_Attack : Player_Action
    {
        [Header("AttackStuffs")]
        [SerializeField] private float playerAttackAnimationDuration;
        [SerializeField, Range(0, 1)] private float playerAttackVolMult = 0.5f;
        [SerializeField] private Sprite[] playerAttackAnimFrames;
        [SerializeField] private UI_Element playerAttackFab;
        [SerializeField] private AudioClip attackSound;
        private UI_Element curPAttack;
        private EnemyObject curEnemy;

        public override IEnumerator ActionResult()
        {
            float t = 0;
            int i = 0;
            float perc = playerAttackAnimationDuration / (float)playerAttackAnimFrames.Length;
            if (Object_Factory.TryGetObject("EnemyObject", out GameObject go))
            {
                curEnemy = go.GetComponent<EnemyObject>();
                Debug.Log("Player Attacked!");
                curPAttack = Object_Factory.SpawnObject(playerAttackFab.gameObject, "PlayerAttack", go.transform).GetComponent<UI_Element>();
                curPAttack.Images[0].sprite = playerAttackAnimFrames[0];

                Audio_Manager.AM.Play2DSFX(attackSound, 1, 1, playerAttackVolMult, false);
                Textbox_String.JumpToEndOfTextbox = true;

                while (t <= playerAttackAnimationDuration)
                {
                    t += Time.deltaTime;
                    //do the animation
                    if (t >= perc)
                    {
                        t = 0;
                        i++;

                        if (i < playerAttackAnimFrames.Length)
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
                            //complete animation if there is no next frame
                            Object_Factory.RemoveObject("PlayerAttack");
                            yield break;
                        }
                    }
                    yield return null;
                }
            }
            else
            {
                Debug.LogWarning("Player Attack Action couldn't find an enemy to hit!");
            }
        }
    }
}
