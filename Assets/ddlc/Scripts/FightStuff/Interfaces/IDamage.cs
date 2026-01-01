
namespace RenCSharp.Combat.Interfaces
{
    public interface IDamage 
    {
        /// <summary>
        /// Take Damage from an attack.
        /// </summary>
        /// <param name="f">The amount of damage that should be inflicted.</param>
        /// <param name="DoT">Whether or not the damage is being dealt over time, or in bulk
        public void TakeDamage(float f, bool DoT);
        /// <summary>
        /// For making funny enemy scaling in difficulty
        /// </summary>
        /// <returns>Percentage resistance to damage</returns>
        public float Resistance();
    }
}
