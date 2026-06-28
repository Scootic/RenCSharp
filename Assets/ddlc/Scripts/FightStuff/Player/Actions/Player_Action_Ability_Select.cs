using System.Collections;
using UnityEngine;
using RenCSharp.EXPERIMENTAL;
using System.Collections.Generic;
namespace RenCSharp.Combat.Player
{
    public class Player_Action_Ability_Select : Player_Action
    {
        [Header("Ability chicanery")]
        [SerializeField] private Player_Ability[] allAbilities;
        [SerializeField] private Transform abilityHolder;
        [SerializeField] private Player_Action_Handler pah;
        [SerializeField] private UI_Element[] abilitySpriters;
        [SerializeField,Min(1)] private int yJump = 3;
        [Header("FlickThroughMenu")]
        [SerializeField] private AudioClip flickSound;
        [SerializeField, Min(0)] private float flickThroughTime = 0.1f;
        [SerializeField, Range(0, 1)] private float flickVolume = 0.3f;
        private int curAbilityIndex, curGOIndex, activeGOs;
        private bool selectedAnAbility, flickThrough;
        private Sprite[] ogSprites;
        private Player_Ability curAbility;
        public override IEnumerator ActionResult()
        {
            curAbilityIndex = 0;
            curGOIndex = 0;
            activeGOs = 0;
            selectedAnAbility = false;
            flickThrough = false;
            abilityHolder.gameObject.SetActive(true);
            Player_Input.Movement += ScrollThroughAbilities;
            Player_Input.Attack += SelectAbility;

            ogSprites = new Sprite[allAbilities.Length];
            for (int i = 0; i < ogSprites.Length; i++)
            {
                ogSprites[i] = abilitySpriters[i].Images[0].sprite;
            }
            abilitySpriters[curAbilityIndex].Images[0].sprite = selectedImage;

            for (int i = 0; i < allAbilities.Length; i++)
            {
                bool unlocked = AbilityUnlocked(allAbilities[i]);
                allAbilities[i].gameObject.SetActive(unlocked);
                abilitySpriters[i].gameObject.SetActive(unlocked);
                if (unlocked) { activeGOs++; }
            }

            while (!selectedAnAbility)
            {
                //idle until we've selected a stinkin' ability.
                yield return null;
            }

            pah.SetCurrentPlayerAbility(allAbilities[curAbilityIndex]);
            abilityHolder.gameObject.SetActive(false);
            Player_Input.Movement -= ScrollThroughAbilities;
            Player_Input.Attack -= SelectAbility;
        }

        private void SelectAbility()
        {
            if (AbilityUnlocked(allAbilities[curAbilityIndex]))
            {
                if (curAbility != null) { curAbility.Current = false; }
                Debug.Log("Selected an ability: " + allAbilities[curAbilityIndex].gameObject.name);
                abilitySpriters[curAbilityIndex].Images[0].sprite = ogSprites[curAbilityIndex];
                curAbility = allAbilities[curAbilityIndex];
                curAbility.Current = true;
                //these bottom two SHOULD be unnecessary. and yet, here we are.
                curAbility.Fighting = true;
                curAbility.PlayerTurn = false;
                selectedAnAbility = true;
            }
            else
            {
                Debug.LogWarning("Player is trying to select an ability that is not yet unlocked! Somehow!");
            }
        }

        private bool AbilityUnlocked(Player_Ability pa)
        {
            int reqBit = pa.RequiredBit;
            if ((reqBit & Flag_Manager.GetFlag("PlayerAbilities")) == reqBit) 
            {
                Debug.Log("Ability: " + pa.gameObject.name + " is unlocked!");
                return true; 
            }
            Debug.Log("Ability: " + pa.gameObject.name + " is not unlocked.");
            return false;
        }

        private void ScrollThroughAbilities(Vector2 v2)
        {
            if (flickThrough) return;
            flickThrough = true;
            StartCoroutine(FlickThrough());
            if (curAbilityIndex < 0 || curAbilityIndex >= abilitySpriters.Length) 
            {
                Debug.LogWarning("Player had a really fucked up ability index, setting it back to 0.");
                curAbilityIndex = 0;
            }
            abilitySpriters[curAbilityIndex].Images[0].sprite = ogSprites[curAbilityIndex];

            if (v2.x >= 1)
            {
                curGOIndex++;
                if (curGOIndex >= allAbilities.Length) curGOIndex = 0;
                while (!allAbilities[curGOIndex].gameObject.activeInHierarchy) 
                { 
                    curGOIndex++;
                    if (curGOIndex >= allAbilities.Length) curGOIndex = 0;
                }
                
            }
            else if (v2.x <= -1)
            {
                curGOIndex--;
                if (curGOIndex < 0) curGOIndex = allAbilities.Length - 1;
                while (!allAbilities[curGOIndex].gameObject.activeInHierarchy) 
                { 
                    curGOIndex--;
                    if (curGOIndex < 0) curGOIndex = allAbilities.Length - 1;
                }
            }
            else if (v2.y >= 1 && activeGOs > yJump)
            {
                curGOIndex -= yJump;
                if (curGOIndex >= activeGOs) curGOIndex += activeGOs;
                while (!allAbilities[curGOIndex].gameObject.activeInHierarchy) curGOIndex--;
            }
            else if (v2.y <= -1 && activeGOs > yJump) 
            {
                curGOIndex += yJump;
                if (curGOIndex < 0) curGOIndex = activeGOs - curGOIndex;
                while (!allAbilities[curGOIndex].gameObject.activeInHierarchy) curGOIndex++;
            }

            Debug.Log("current gameobject index: " + curGOIndex);
            curAbilityIndex = curGOIndex;
            abilitySpriters[curAbilityIndex].Images[0].sprite = selectedImage;
            Audio_Manager.AM.Play2DSFX(flickSound, 0.9f, 1.1f, flickVolume);
        }

        private IEnumerator FlickThrough()
        {
            yield return new WaitForSeconds(flickThroughTime);
            flickThrough = false;
        }
    }
}
