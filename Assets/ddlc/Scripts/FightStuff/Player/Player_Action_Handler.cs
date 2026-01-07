using System.Collections;
using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public sealed class Player_Action_Handler : MonoBehaviour
    {
        [SerializeField] private Player_Action[] playerActions;
        [SerializeField,Min(0)] private float flickThroughMenuTime = 0.1f;
        [SerializeField, Range(0,1)] private float actionSelectVolume = 1;
        [SerializeField] private AudioClip actionSelectSound;
        private Player_Action curAction;
        private int curIndex;
        private Player_Ability curAbility;
        private bool lockedIn, flickThroughMenu;
        public bool PlayerActionLockedIn => lockedIn;
        public Player_Action CurrentPlayerAction => curAction;
        public Player_Ability CurrentAbility => curAbility;

        public void StartPlayerTurn()
        {
            lockedIn = false;
            curIndex = 0;
            flickThroughMenu = false;
            if (curAbility != null) { curAbility.PlayerTurn = true; curAbility.Fighting = true; }
            SelectAnAction(curIndex); //default to first possible action whenever a turn has started
            Player_Input.Movement += ScrollThroughActions;
            Player_Input.Attack += LockInAction;
        }

        public void EndPlayerTurn()
        {
            if (curAbility != null) curAbility.PlayerTurn = false;
        }

        public void EndFight()
        {
            if(curAbility != null) curAbility.Fighting = false;
        }

        public void SetCurrentPlayerAbility(Player_Ability pa)
        {
            Player_Input.Ability = null; //clear out previous ability, if applicable
            curAbility = pa;
            Player_Input.Ability += pa.FireAbility;
        }

        private void ScrollThroughActions(Vector2 guh)
        {
            if (flickThroughMenu == true) return;
            flickThroughMenu = true;
            StartCoroutine(FlickThroughMenu());     
            //we don' care 'bout y tf
            if (guh.x >= 1)
            {
                curIndex++;
                if (curIndex >= playerActions.Length) curIndex = 0;
            }
            else if(guh.x <= -1)
            {
                curIndex--;
                if(curIndex < 0) curIndex = playerActions.Length - 1;
            }

            SelectAnAction(curIndex);
        }

        private void SelectAnAction(int index)
        {
            if (curAction != null) curAction.OnActionDeselect();
            curAction = playerActions[index];
            curAction.OnActionSelect();
            Audio_Manager.AM.Play2DSFX(actionSelectSound, 0.9f, 1.1f, actionSelectVolume);
        }

        private void LockInAction()
        {
            lockedIn = true;
            Player_Input.Movement -= ScrollThroughActions;
            Player_Input.Attack -= LockInAction;
        }

        private IEnumerator FlickThroughMenu()
        {
            yield return new WaitForSeconds(flickThroughMenuTime);
            flickThroughMenu = false;
        }
    }
}
