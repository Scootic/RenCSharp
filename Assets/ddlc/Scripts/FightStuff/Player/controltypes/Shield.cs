using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public class Shield : ControlType
    {
        [SerializeField] private GameObject playerShieldObj;
        [SerializeField] private Sprite shieldVisual;
        [SerializeField,Min(0.01f)] private float shieldTurnSpeed = 1;
        private GameObject curShield;
        protected override void MovementEffect(Vector2 dir)
        {
            Quaternion delayTurn = Quaternion.Lerp(curShield.transform.localRotation, TrigHelper.GetQuaternion(dir, -90), Time.deltaTime * shieldTurnSpeed);
            curShield.transform.localRotation = delayTurn;
        }

        public override void EnterControl()
        {
            base.EnterControl();
            curShield = Object_Factory.SpawnObject(playerShieldObj, "PlayerShield", playerObj.transform);
            curShield.GetComponent<UI_Element>().Images[0].sprite = shieldVisual;
        }

        public override void ExitControl()
        {
            Object_Factory.RemoveObject("PlayerShield");
            base.ExitControl();
        }

        protected override Color PlayerColor()
        {
            return Color.green;
        }

        public override string ToString()
        {
            return "Shield";
        }
    }
}
