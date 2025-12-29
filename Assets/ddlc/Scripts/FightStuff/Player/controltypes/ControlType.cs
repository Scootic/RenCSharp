using System;
using UnityEngine;
using UnityEngine.UI;
namespace RenCSharp.Combat
{
    [Serializable]
    public abstract class ControlType
    {
        protected abstract Color PlayerColor();
        protected Player_Object playerObj;
        /// <summary>
        /// What happens when the game receives player input.
        /// </summary>
        /// <param name="dir">The direction given by Input System</param>
        protected abstract void MovementEffect(Vector2 dir);
        /// <summary>
        /// Occurs when an attack begins.
        /// </summary>
        public virtual void EnterControl()
        {
            if(!Object_Factory.TryGetObject("PlayerObject", out GameObject go)) { Debug.LogWarning("No player obj!"); return; }
            playerObj = go.GetComponent<Player_Object>();
            playerObj.GetComponent<Image>().color = PlayerColor();
            Player_Input.Movement += MovementEffect;
        }
        /// <summary>
        /// Occurs when an attack ends.
        /// </summary>
        public virtual void ExitControl()
        {
            Player_Input.Movement -= MovementEffect;
        }

        public override string ToString()
        {
            return "Null";
        }
    }
}
