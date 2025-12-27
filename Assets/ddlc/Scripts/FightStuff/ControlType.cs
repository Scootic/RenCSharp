using UnityEngine;

namespace RenCSharp.Combat
{
    public abstract class ControlType
    {
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
            if(!Object_Factory.TryGetObject("PlayerObj", out GameObject go)) { Debug.LogWarning("No player obj!"); }
            playerObj = go.GetComponent<Player_Object>();
            Player_Input.Movement += MovementEffect;
        }
        /// <summary>
        /// Occurs when an attack ends.
        /// </summary>
        public virtual void ExitControl()
        {
            Player_Input.Movement -= MovementEffect;
        }
    }
}
