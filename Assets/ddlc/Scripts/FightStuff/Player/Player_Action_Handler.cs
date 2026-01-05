using System.Collections;
using UnityEngine;

namespace RenCSharp.Combat
{
    public sealed class Player_Action_Handler : MonoBehaviour
    {
        [SerializeField] private Player_Action[] playerActions;
        private Player_Action curAction;
        private int curIndex;
        private Player_Ability curAbility;
        private bool lockedIn;
        public bool PlayerActionLockedIn => lockedIn;
        public Player_Action CurrentPlayerAction => curAction;
        public Player_Ability CurrentAbility => curAbility;

        public void StartPlayerTurn()
        {
            lockedIn = false;
            curIndex = 0;
            SelectAnAction(curIndex); //default to first possible action whenever a turn has started
            Player_Input.Movement += ScrollThroughActions;
            Player_Input.Attack += LockInAction;
        }

        public void EndPlayerTurn()
        {
            Player_Input.Movement -= ScrollThroughActions;
            Player_Input.Attack -= LockInAction;
        }

        public void SetCurrentPlayerAbility(Player_Ability pa)
        {
            Player_Input.Ability = null; //clear out previous ability, if applicable
            curAbility = pa;
            Player_Input.Ability += pa.FireAbility;
        }

        private void ScrollThroughActions(Vector2 guh)
        {
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
        }

        private void LockInAction()
        {
            lockedIn = true;
        }
    }
}
