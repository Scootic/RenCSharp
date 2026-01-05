using System.Collections;
using UnityEngine;

namespace RenCSharp.Combat
{
    public class Player_Action_Ability_Select : Player_Action
    {
        [Header("Ability chicanery")]
        [SerializeField] private Player_Ability[] allAbilities;
        [SerializeField] private Transform abilityHolder;
        [SerializeField] private Player_Action_Handler pah;
        [SerializeField] private UI_Element[] abilitySpriters;

        [SerializeField,Min(1)] private int yJump = 3;
        private int curAbilityIndex, curGOIndex, activeGOs;
        private bool selectedAnAbility;
        private Sprite[] ogSprites;
        public override IEnumerator ActionResult()
        {
            curAbilityIndex = 0;
            curGOIndex = 0;
            activeGOs = 0;
            selectedAnAbility = false;
            abilityHolder.gameObject.SetActive(true);
            Player_Input.Movement += ScrollThroughAbilities;
            Player_Input.Attack += SelectAbility;

            ogSprites = new Sprite[allAbilities.Length];
            for (int i = 0; i < ogSprites.Length; i++)
            {
                ogSprites[i] = abilitySpriters[i].Images[0].sprite;
            }
            abilitySpriters[curAbilityIndex].Images[0].sprite = selectedImage;

            foreach(Player_Ability pa in allAbilities)
            {
                pa.gameObject.SetActive(AbilityUnlocked(pa));
                if (pa.gameObject.activeInHierarchy) activeGOs++;
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
                abilitySpriters[curAbilityIndex].Images[0].sprite = ogSprites[curAbilityIndex];
                selectedAnAbility = true;
            }
            else
            {
                Debug.LogWarning("Player is trying to select an ability that is not yet unlocked!");
            }
        }

        private bool AbilityUnlocked(Player_Ability pa)
        {
            int reqBit = pa.RequiredBit;
            if ((reqBit & Flag_Manager.GetFlag("PlayerAbilities")) == reqBit) return true;
            return false;
        }

        private void ScrollThroughAbilities(Vector2 v2)
        {
            abilitySpriters[curAbilityIndex].Images[0].sprite = ogSprites[curAbilityIndex];

            if (v2.x >= 1)
            {
                curGOIndex++;
                if (curGOIndex >= activeGOs) curGOIndex = 0;
                while (!abilityHolder.GetChild(curGOIndex).gameObject.activeInHierarchy) curGOIndex++;
                
            }
            else if (v2.x <= -1)
            {
                curGOIndex--;
                if (curGOIndex < 0) curGOIndex = activeGOs - 1;
                while (!abilityHolder.GetChild(curGOIndex).gameObject.activeInHierarchy) curGOIndex--;
            }
            else if (v2.y >= 1)
            {
                curGOIndex += yJump;
                if (curGOIndex >= activeGOs) curGOIndex -= activeGOs;
                while (!abilityHolder.GetChild(curGOIndex).gameObject.activeInHierarchy) curGOIndex++;
            }
            else if (v2.y <= -1) 
            {
                curGOIndex -= yJump;
                if (curGOIndex < 0) curGOIndex = activeGOs + curGOIndex;
                while (!abilityHolder.GetChild(curGOIndex).gameObject.activeInHierarchy) curGOIndex--;
            }

            Debug.Log("current gameobject index: " + curGOIndex);
            curAbilityIndex = curGOIndex;
            abilitySpriters[curAbilityIndex].Images[0].sprite = selectedImage;
        }
    }
}
