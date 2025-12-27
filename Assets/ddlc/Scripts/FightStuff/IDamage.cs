using UnityEngine;

namespace RenCSharp.Combat
{
    public interface IDamage 
    {
        /// <summary>
        /// Take Damage from an attack.
        /// </summary>
        /// <param name="f">The amount of damage that should be inflicted.</param>
        public void TakeDamage(float f);
        /// <summary>
        /// For making funny enemy scaling in difficulty
        /// </summary>
        /// <returns>Percentage resistance to damage</returns>
        public float Resistance();
    }
}
