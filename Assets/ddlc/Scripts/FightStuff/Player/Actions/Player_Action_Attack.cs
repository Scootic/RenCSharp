using RenCSharp.EXPERIMENTAL;
using System.Collections;
using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Player_Action_Attack : Player_Action
    {
        [Header("AttackStuffs")]
        [SerializeField] private float playerAttackAnimationDuration;
        [SerializeField, Range(0, 1)] private float playerAttackVolMult = 0.5f;
        [SerializeField] private Sprite[] playerAttackAnimFrames;
        [SerializeField] private Animated_Image_Handler playerAttackFab;
        [SerializeField] private AudioClip attackSound;
        [SerializeField] private float attackDamage;
        [SerializeField] private string playerDamageFlag = "PlayerDamage";
        [SerializeField, Tooltip("Disable if you want to manually set attack damage for debug/cheating purposes.")] private bool grabAttackDamageFromFlags = true;
        private Animated_Image_Handler curPAttack;

        public override IEnumerator ActionResult()
        {
            if (Object_Factory.TryGetObject("EnemyObject", out GameObject go))
            {
                float divTwo = playerAttackAnimationDuration * 0.5f;
                float secondsPerFrame = playerAttackAnimationDuration / (float)playerAttackAnimFrames.Length;
                //Debug.Log("Player Attacked!");
                curPAttack = Object_Factory.SpawnObject(playerAttackFab.gameObject, "PlayerAttack", go.transform).GetComponent<Animated_Image_Handler>();
                curPAttack.ReceiveAnimationInformation(playerAttackAnimFrames, secondsPerFrame);

                Audio_Manager.AM.Play2DSFX(attackSound, 1, 1, playerAttackVolMult, false);
                Textbox_String.JumpToEndOfTextbox = true;

                yield return new WaitForSeconds(divTwo);
                if(grabAttackDamageFromFlags) attackDamage = (float)Flag_Manager.GetFlag(playerDamageFlag);
                Event_Bus.TryFireDoubleObjEvent("EnemyTakeDamage", (object)attackDamage, (object)false);
                yield return new WaitForSeconds(divTwo);
                Object_Factory.RemoveObject("PlayerAttack");
            }
            else
            {
                Debug.LogWarning("Player Attack Action couldn't find an enemy to hit! Frick!");
            }
        }
    }
}
